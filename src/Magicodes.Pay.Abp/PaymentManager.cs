// ======================================================================
// 
//           Copyright (C) 2019-2030 湖南心莱信息科技有限公司
//           All rights reserved
// 
//           filename : PaymentManager.cs
//           description :
// 
//           created by 雪雁 at  2019-10-26 10:45
//           文档官网：https://docs.xin-lai.com
//           公众号教程：麦扣聊技术
//           QQ群：85318032（编程交流）
//           Blog：http://www.cnblogs.com/codelove/
// 
// ======================================================================

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.Configuration;
using Abp.Dependency;
using Abp.Domain.Uow;
using Abp.Json;
using Abp.Runtime.Session;
using Abp.UI;
using Castle.Core.Logging;
using Magicodes.Pay.Abp.Callbacks;
using Magicodes.Pay.Abp.Dto;
using Magicodes.Pay.Abp.Registers;
using Magicodes.Pay.Abp.Services;
using Magicodes.Pay.Abp.TransactionLogs;
using Magicodes.Pay.Notify;
using Magicodes.Pay.Notify.Builder;
using Magicodes.Pay.Notify.Models;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;

namespace Magicodes.Pay.Abp
{
    /// <summary>
    ///     支付管理器
    /// </summary>
    public class PaymentManager : IPaymentManager
    {
        private readonly IIocManager _iocManager;

        /// <summary>
        /// </summary>
        /// <param name="iocManager"></param>
        public PaymentManager(IIocManager iocManager)
        {
            _iocManager = iocManager;
            Logger = NullLogger.Instance;
        }

        /// <summary>
        /// </summary>
        protected List<IPaymentCallbackAction> PaymentCallbackActions { get; private set; }

        /// <summary>
        /// </summary>
        public ILogger Logger { get; set; }

        /// <summary>
        /// </summary>
        protected List<IPaymentRegister> PaymentRegisters { get; private set; }

        /// <summary>
        /// 支付服务
        /// </summary>
        protected List<IToPayService> ToPayServices { get; private set; }


        /// <summary>
        ///     Implementors should perform any initialization logic.
        /// </summary>
        public void Initialize()
        {
            PaymentRegisters = _iocManager.ResolveAll<IPaymentRegister>()?.ToList();
            PaymentCallbackActions = _iocManager.ResolveAll<IPaymentCallbackAction>()?.ToList();
            ToPayServices = _iocManager.ResolveAll<IToPayService>()?.ToList();

            //日志函数
            void LogAction(string tag, string message)
            {
                if (tag.Equals("error", StringComparison.CurrentCultureIgnoreCase))
                    Logger.Error(message);
                else
                    Logger.Debug(message);
            }


            if (PaymentRegisters != null)
                foreach (var action in PaymentRegisters)
                    //
                    action.Build(LogAction).Wait();

            PayNotifyConfig(LogAction);
        }

        /// <summary>
        ///     执行支付通知
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public virtual async Task<string> ExecPayNotifyAsync(PayNotifyInput input)
        {
            var action = PaymentRegisters.FirstOrDefault(p =>
                input.Provider.Equals(p.Key, StringComparison.OrdinalIgnoreCase));
            if (action == null) throw new UserFriendlyException($"Provider：{input.Provider} 不存在，请确认是否已注册相关逻辑！");

            using (_iocManager.CreateScope())
            {
                var abpSession = _iocManager.Resolve<IAbpSession>();
                int? tenantId = null;
                if (!string.IsNullOrEmpty(input.TenantId))
                {
                    tenantId = Convert.ToInt32(input.TenantId);
                }
                using (abpSession.Use(tenantId, null))
                { 
                    var result = await action.ExecPayNotifyAsync(input);
                    if (result == null)
                    {
                        throw new UserFriendlyException("ExecPayNotifyAsync必须处理并返回支付参数！");
                    }
                    if (string.IsNullOrWhiteSpace(result.BusinessParams))
                    {
                        throw new UserFriendlyException("请配置自定义参数！");
                    }
                    //目前仅用支付参数的业务字段存储key，自定义数据在交易日志的CustomData中
                    var key = result.BusinessParams.Contains("{") ? result.BusinessParams.FromJsonString<JObject>()["key"]?.ToString() : result.BusinessParams;
                    await ExecuteCallback(key, result.OutTradeNo, result.TradeNo, result.TotalFee);
                    return result.SuccessResult?.ToString();
                }
            }
        }

        /// <summary>
        ///     是否注册回调逻辑
        /// </summary>
        /// <param name="paymentCallbackAction"></param>
        /// <returns></returns>
        public virtual async Task<bool> IsRegisterCallbackAction(IPaymentCallbackAction paymentCallbackAction)
        {
            return await Task.FromResult(PaymentCallbackActions.Contains(paymentCallbackAction) ||
                                         PaymentCallbackActions.Any(p => p.Key == paymentCallbackAction.Key));
        }

        /// <summary>
        ///     注册回调逻辑
        /// </summary>
        /// <param name="paymentCallbackAction"></param>
        /// <returns></returns>
        public virtual async Task RegisterCallbackAction(IPaymentCallbackAction paymentCallbackAction)
        {
            if (!await IsRegisterCallbackAction(paymentCallbackAction))
                PaymentCallbackActions.Add(paymentCallbackAction);
            await Task.FromResult(0);
        }


        /// <summary>
        /// 获取所有
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<IPaymentCallbackAction>> GitAllCallbackActions()
        {
            return await Task.FromResult(PaymentCallbackActions);
        }

        /// <summary>
        ///     执行回调逻辑
        /// </summary>
        /// <param name="key">支付业务关键字</param>
        /// <param name="outTradeNo">商户系统的订单号</param>
        /// <param name="totalFee">金额（单位：分）</param>
        /// <param name="transactionId">微信支付订单号</param>
        /// <returns></returns>
        public async Task ExecuteCallback(string key, string outTradeNo, string transactionId, decimal totalFee)
        {
            using (var obj = _iocManager.ResolveAsDisposable<TransactionLogHelper>())
            {
                //更新交易日志
                await obj.Object.UpdateAsync(outTradeNo, transactionId, async (unitOfWork, logInfo) =>
                {
                    var data = logInfo.CustomData.FromJsonString<JObject>();
                    Logger?.Info($"正在执行【{key}】回调逻辑。data:{data?.ToJsonString()}");



                    if (!decimal.Equals(logInfo.Currency.CurrencyValue, totalFee))
                        throw new UserFriendlyException(
                            $"支付金额不一致：要求支付金额为：{logInfo.Currency.CurrencyValue}，实际支付金额为：{totalFee}");

                    var paymentCallbackAction = PaymentCallbackActions?.FirstOrDefault(p => p.Key == key);
                    if (paymentCallbackAction == null)
                        throw new UserFriendlyException($"Key：{key} 不存在，请使用Register方法进行注册");

                    await paymentCallbackAction.Process(unitOfWork, logInfo);
                });
            }
        }

        /// <summary>
        /// 获取支付服务
        /// </summary>
        /// <param name="payChannel"></param>
        /// <returns></returns>
        public async Task<IToPayService> GetPayService(PayChannels payChannel)
        {
            return await Task.FromResult(ToPayServices.FirstOrDefault(p => p.PayChannel == payChannel));
        }

        /// <summary>
        ///     卸载
        /// </summary>
        /// <param name="paymentCallbackAction"></param>
        /// <returns></returns>
        public async Task UnRegisterCallbackAction(IPaymentCallbackAction paymentCallbackAction)
        {
            if (await IsRegisterCallbackAction(paymentCallbackAction))
                PaymentCallbackActions.Remove(PaymentCallbackActions.First(p => p.Key == paymentCallbackAction.Key));
            await Task.FromResult(0);
        }


        /// <summary>
        ///     支付回调配置
        /// </summary>
        /// <param name="logAction"></param>
        public void PayNotifyConfig(Action<string, string> logAction)
        {
            if (_iocManager.IsRegistered<PayNotifyController>()) return;

            //注册支付回调控制器
            _iocManager.Register<PayNotifyController>(DependencyLifeStyle.Transient);

            //支付回调设置
            PayNotifyBuilder
                .Create()
                //设置日志记录
                .WithLoggerAction(logAction).WithPayNotifyFunc(async input => await ExecPayNotifyAsync(input)).Build();
        }
    }
}
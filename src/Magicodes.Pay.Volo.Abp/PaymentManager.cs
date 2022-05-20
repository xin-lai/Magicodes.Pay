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
using Magicodes.Pay.Volo.Abp.Callbacks;
using Magicodes.Pay.Volo.Abp.Registers;
using Magicodes.Pay.Volo.Abp.Services;
using Magicodes.Pay.Volo.Abp.TransactionLogs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp;
using Magicodes.Pay.Notify.Models;
using Volo.Abp.MultiTenancy;
using Magicodes.Pay.Notify;
using Volo.Abp.Json;
using Magicodes.Pay.Notify.Builder;

namespace Magicodes.Pay.Volo.Abp
{
    /// <summary>
    ///     支付管理器
    /// </summary>
    public class PaymentManager : IPaymentManager
    {
        private readonly IServiceProvider serviceProvider;
        private readonly IJsonSerializer jsonSerializer;

        /// <summary>
        /// </summary>
        /// <param name="serviceProvider"></param>
        public PaymentManager(IServiceProvider serviceProvider, ILogger<PaymentManager> logger, IJsonSerializer jsonSerializer)
        {
            this.serviceProvider = serviceProvider;
            Logger = logger;
            this.jsonSerializer = jsonSerializer;
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
            PaymentRegisters = serviceProvider.GetServices<IPaymentRegister>().ToList();
            if (PaymentRegisters != null)
                foreach (var action in PaymentRegisters)
                    action.Build(logAction);

            PaymentCallbackActions = serviceProvider.GetServices<IPaymentCallbackAction>().ToList();
            ToPayServices = serviceProvider.GetServices<IToPayService>().ToList();

            //日志函数
            void logAction(string tag, string message)
            {
                if (tag.Equals("error", StringComparison.CurrentCultureIgnoreCase))
                    Logger.LogError(message);
                else
                    Logger.LogDebug(message);
            }

            //支付回调设置
            PayNotifyBuilder
                .Create()
                //设置日志记录
                .WithLoggerAction(logAction).WithPayNotifyFunc(async input => await ExecPayNotifyAsync(input)).Build();

        }

        /// <summary>
        ///     执行支付通知
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public virtual async Task<string?> ExecPayNotifyAsync(PayNotifyInput input)
        {
            var action = PaymentRegisters.FirstOrDefault(p =>
                input.Provider.Equals(p.Key, StringComparison.OrdinalIgnoreCase));
            if (action == null) throw new BusinessException($"Provider：{input.Provider} 不存在，请确认是否已注册相关逻辑！");

            using (serviceProvider.CreateScope())
            {
                var tenant = serviceProvider.GetService<ICurrentTenant>();
                Guid? tenantId = null;
                if (!string.IsNullOrEmpty(input.TenantId))
                {
                    tenantId = Guid.Parse(input.TenantId);
                }
                using (tenant?.Change(tenantId))
                {
                    var result = await action.ExecPayNotifyAsync(input);
                    if (result == null)
                    {
                        throw new BusinessException("ExecPayNotifyAsync必须处理并返回支付参数！");
                    }
                    if (string.IsNullOrWhiteSpace(result.BusinessParams))
                    {
                        throw new BusinessException("请配置自定义参数！");
                    }
                    //目前仅用支付参数的业务字段存储key，自定义数据在交易日志的CustomData中
                    var key = result.BusinessParams.Contains("{") ? jsonSerializer.Deserialize<JObject>(result.BusinessParams)["key"]?.ToString() : result.BusinessParams;
                    if (!string.IsNullOrWhiteSpace(key))
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
            var transactionLogHelper = serviceProvider.GetRequiredService<TransactionLogHelper>();
            {
                //更新交易日志
                await transactionLogHelper.UpdateAsync(outTradeNo, transactionId, async (unitOfWork, logInfo) =>
                {
                    var data = jsonSerializer.Deserialize<JObject>(logInfo.CustomData);
                    Logger?.LogInformation($"正在执行【{key}】回调逻辑。data:{logInfo.CustomData}");

                    if (!decimal.Equals(logInfo.Amount, totalFee))
                        throw new BusinessException(message:
                            $"支付金额不一致：要求支付金额为：{logInfo.Amount}，实际支付金额为：{totalFee}");

                    var paymentCallbackAction = PaymentCallbackActions?.FirstOrDefault(p => p.Key == key);
                    if (paymentCallbackAction == null)
                        throw new BusinessException(message: $"Key：{key} 不存在，请使用Register方法进行注册");

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
    }
}
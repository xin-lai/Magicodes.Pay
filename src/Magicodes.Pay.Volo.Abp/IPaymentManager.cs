// ======================================================================
// 
//           Copyright (C) 2019-2030 湖南心莱信息科技有限公司
//           All rights reserved
// 
//           filename : IPaymentConfigManager.cs
//           description :
// 
//           created by 雪雁 at  -- 
//           文档官网：https://docs.xin-lai.com
//           公众号教程：麦扣聊技术
//           QQ群：85318032（编程交流）
//           Blog：http://www.cnblogs.com/codelove/
// 
// ======================================================================

using System.Collections.Generic;
using System.Threading.Tasks;
using Magicodes.Pay.Notify.Models;
using Magicodes.Pay.Volo.Abp.Callbacks;
using Magicodes.Pay.Volo.Abp.Services;
using Magicodes.Pay.Volo.Abp.TransactionLogs;
using Volo.Abp.DependencyInjection;

namespace Magicodes.Pay.Volo.Abp
{
    /// <summary>
    /// 支付配置管理器
    /// </summary>
    public interface IPaymentManager : ISingletonDependency
    {
        /// <summary>
        /// 执行支付回调通知
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<string> ExecPayNotifyAsync(PayNotifyWithGuidInput input);

        /// <summary>
        /// 是否注册回调逻辑
        /// </summary>
        /// <param name="paymentCallbackAction"></param>
        /// <returns></returns>
        Task<bool> IsRegisterCallbackAction(IPaymentCallbackAction paymentCallbackAction);

        /// <summary>
        /// 注册回调逻辑
        /// </summary>
        /// <param name="paymentCallbackAction"></param>
        /// <returns></returns>
        Task RegisterCallbackAction(IPaymentCallbackAction paymentCallbackAction);

        /// <summary>
        /// 卸载回调逻辑
        /// </summary>
        /// <param name="paymentCallbackAction"></param>
        /// <returns></returns>
        Task UnRegisterCallbackAction(IPaymentCallbackAction paymentCallbackAction);

        /// <summary>
        /// 获取所有
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<IPaymentCallbackAction>> GitAllCallbackActions();

        /// <summary>
        /// 执行回调逻辑
        /// </summary>
        /// <param name="key">支付业务关键字</param>
        /// <param name="outTradeNo">商户系统的订单号</param>
        /// <param name="totalFee">金额（单位：元）</param>
        /// <param name="transactionId">微信支付订单号</param>
        /// <returns></returns>
        Task ExecuteCallback(string key, string outTradeNo, string transactionId, decimal totalFee);

        /// <summary>
        /// 获取支付服务
        /// </summary>
        /// <param name="payChannel"></param>
        /// <returns></returns>
        Task<IToPayService> GetPayService(PayChannels payChannel);
    }
}
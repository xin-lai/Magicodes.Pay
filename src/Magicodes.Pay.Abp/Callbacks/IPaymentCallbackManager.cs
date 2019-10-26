using System.Threading.Tasks;
using Abp.Dependency;

namespace Magicodes.Pay.Abp.Callbacks
{
    /// <summary>
    /// 支付回调管理器
    /// </summary>
    public interface IPaymentCallbackManager : ISingletonDependency
    {
        /// <summary>
        /// 注册回调逻辑
        /// </summary>
        /// <param name="paymentCallbackAction"></param>
        /// <returns></returns>
        Task Register(IPaymentCallbackAction paymentCallbackAction);

        /// <summary>
        /// 是否注册
        /// </summary>
        /// <param name="paymentCallbackAction"></param>
        /// <returns></returns>
        Task<bool> IsRegister(IPaymentCallbackAction paymentCallbackAction);

        /// <summary>
        /// 卸载
        /// </summary>
        /// <param name="paymentCallbackAction"></param>
        /// <returns></returns>
        Task UnRegister(IPaymentCallbackAction paymentCallbackAction);

        /// <summary>
        /// 执行回调逻辑
        /// </summary>
        /// <param name="key">支付业务关键字</param>
        /// <param name="outTradeNo">商户系统的订单号</param>
        /// <param name="totalFee">金额（单位：分）</param>
        /// <param name="transactionId">微信支付订单号</param>
        /// <returns></returns>
        Task ExecuteCallback(string key, string outTradeNo, string transactionId, int totalFee);
    }
}
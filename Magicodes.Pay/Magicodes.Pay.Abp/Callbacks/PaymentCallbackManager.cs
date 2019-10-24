using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Abp.Json;
using Abp.Runtime.Session;
using Abp.UI;
using Castle.Core.Logging;
using Magicodes.Pay.Abp.TransactionLogs;
using Newtonsoft.Json.Linq;

namespace Magicodes.Pay.Abp.Callbacks
{
    /// <summary>
    /// 支付回调管理
    /// </summary>
    public class PaymentCallbackManager : IPaymentCallbackManager
    {
        protected List<IPaymentCallbackAction> PaymentCallbackActions { get; set; } =
            new List<IPaymentCallbackAction>();

        /// <summary>
        /// 
        /// </summary>
        public ILogger Logger { get; set; }

        private readonly TransactionLogHelper _transactionLogHelper;

        /// <summary>
        /// 
        /// </summary>
        public IAbpSession AbpSession { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="transactionLogHelper"></param>
        public PaymentCallbackManager(TransactionLogHelper transactionLogHelper)
        {
            _transactionLogHelper = transactionLogHelper;
            Logger = NullLogger.Instance;
            AbpSession = NullAbpSession.Instance;
        }

        /// <summary>
        /// 注册回调逻辑
        /// </summary>
        /// <param name="paymentCallbackAction"></param>
        /// <returns></returns>
        public async Task Register(IPaymentCallbackAction paymentCallbackAction)
        {
            if (!await IsRegister(paymentCallbackAction))
                PaymentCallbackActions.Add(paymentCallbackAction);
            await Task.FromResult(0);
        }

        /// <summary>
        /// 是否注册
        /// </summary>
        /// <param name="paymentCallbackAction"></param>
        /// <returns></returns>
        public async Task<bool> IsRegister(IPaymentCallbackAction paymentCallbackAction)
        {
            return await Task.FromResult(PaymentCallbackActions.Contains(paymentCallbackAction) || PaymentCallbackActions.Any(p => p.Key == paymentCallbackAction.Key));
        }

        /// <summary>
        /// 卸载
        /// </summary>
        /// <param name="paymentCallbackAction"></param>
        /// <returns></returns>
        public async Task UnRegister(IPaymentCallbackAction paymentCallbackAction)
        {
            if (await IsRegister(paymentCallbackAction))
            {
                PaymentCallbackActions.Remove(PaymentCallbackActions.First(p => p.Key == paymentCallbackAction.Key));
            }
            await Task.FromResult(0);
        }

        /// <summary>
        /// 执行回调逻辑
        /// </summary>
        /// <param name="key">支付业务关键字</param>
        /// <param name="outTradeNo">商户系统的订单号</param>
        /// <param name="totalFee">金额（单位：分）</param>
        /// <param name="transactionId">微信支付订单号</param>
        /// <returns></returns>
        public async Task ExecuteCallback(string key, string outTradeNo, string transactionId, int totalFee)
        {
            //更新交易日志
            await _transactionLogHelper.UpdateAsync(outTradeNo, transactionId, async (unitOfWork, logInfo) =>
            {
                var data = logInfo.CustomData.FromJsonString<JObject>();
                Logger?.Info($"正在执行【{key}】回调逻辑。data:{data?.ToJsonString()}");

                if (decimal.Equals(logInfo.Currency.CurrencyValue * 100, totalFee))
                {
                    throw new UserFriendlyException(
                        $"支付金额不一致：要求支付金额为：{logInfo.Currency.CurrencyValue}，实际支付金额为：{totalFee}");
                }

                var paymentCallbackAction = PaymentCallbackActions.Find(p => p.Key == key);
                if (paymentCallbackAction == null)
                {
                    throw new UserFriendlyException($"Key：{key} 不存在，请使用Register方法进行注册");
                }

                await paymentCallbackAction.Process(unitOfWork, logInfo);

            });
        }
    }
}

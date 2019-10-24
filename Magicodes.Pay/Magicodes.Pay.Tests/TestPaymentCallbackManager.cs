using System.Threading.Tasks;
using Abp;
using Abp.Dependency;
using Abp.Domain.Repositories;
using Abp.Json;
using Abp.Runtime.Session;
using Abp.UI;
using Castle.Core.Logging;
using Magicodes.Admin.Application.Core.Payments.PaymentCallbacks;
using Magicodes.Admin.Application.Core.Payments.TransactionLogs;
using Magicodes.Admin.Core.Authorization.Users;
using Magicodes.Admin.Core.Custom.Logs;
using Newtonsoft.Json.Linq;

namespace Magicodes.Admin.Tests.Custom.Payments
{
    /// <summary>
    /// 支付回调管理
    /// </summary>
    public class TestPaymentCallbackManager : IPaymentCallbackManager
    {
        public ILogger Logger { get; set; }

        private readonly TransactionLogHelper _transactionLogHelper;
        //public UserManager UserManager { get; set; }
        public IAbpSession AbpSession { get; set; }

        private readonly IReceiptCodeGenerator _receiptCodeGenerator;
        private readonly IRepository<PaymentLog, long> _paymentLogRepository;

        /// <summary>
        /// 
        /// </summary>
        public TestPaymentCallbackManager()
        {
            Logger = NullLogger.Instance;
            AbpSession = NullAbpSession.Instance;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="transactionLogHelper"></param>
        /// <param name="paymentLogRepository"></param>
        /// <param name="receiptCodeGenerator"></param>
        public TestPaymentCallbackManager(TransactionLogHelper transactionLogHelper, IRepository<PaymentLog, long> paymentLogRepository, IReceiptCodeGenerator receiptCodeGenerator)
        {
            _transactionLogHelper = transactionLogHelper;
            this._paymentLogRepository = paymentLogRepository;
            _receiptCodeGenerator = receiptCodeGenerator;
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

                
                switch (key)
                {
                    case "缴费支付":
                        {
                            throw new System.NotImplementedException();
                        }
                    default:

                        break;
                }
            });
        }
    }
}

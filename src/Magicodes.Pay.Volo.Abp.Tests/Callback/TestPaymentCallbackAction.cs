using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Magicodes.Pay.Volo.Abp.Callbacks;
using Volo.Abp.Uow;
using Magicodes.Pay.Volo.Abp.TransactionLogs;

namespace Magicodes.Pay.Volo.Abp.Tests.Callback
{
    public class TestPaymentCallbackAction : IPaymentCallbackAction
    {
        /// <summary>
        /// 业务Key
        /// </summary>
        public string Key { get; set; } = "缴费支付";

        /// <summary>
        /// 执行回调
        /// </summary>
        /// <returns></returns>
        public async Task Process(IUnitOfWorkManager unitOfWork, TransactionLog transactionLog)
        {
            var data = transactionLog.CustomData.FromJsonString<JObject>();
            //业务处理

            await Task.FromResult(0);
        }
    }
}

using System.Threading.Tasks;
using Abp.Domain.Uow;
using Abp.Json;
using Magicodes.Pay.Abp.Callbacks;
using Magicodes.Pay.Abp.TransactionLogs;
using Newtonsoft.Json.Linq;

namespace Magicodes.Pay.Tests.Callback
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

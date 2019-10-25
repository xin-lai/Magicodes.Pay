using System.Threading.Tasks;
using Abp.Domain.Uow;
using Abp.Json;
using Abp.UI;
using Magicodes.Pay.Abp.Callbacks;
using Magicodes.Pay.Abp.TransactionLogs;
using Newtonsoft.Json.Linq;

namespace Magicodes.Pay.Tests.Callback
{
    public class TestPaymentCallbackErrorAction : IPaymentCallbackAction
    {
        /// <summary>
        /// 业务Key
        /// </summary>
        public string Key { get; set; } = "缴费支付";

        /// <summary>
        /// 执行回调
        /// </summary>
        /// <returns></returns>
        public Task Process(IUnitOfWorkManager unitOfWork, TransactionLog transactionLog)
        {
            throw new UserFriendlyException("支付报错！");
        }
    }
}

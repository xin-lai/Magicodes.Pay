using System.Threading.Tasks;
using Magicodes.Pay.Volo.Abp.Callbacks;
using Magicodes.Pay.Volo.Abp.TransactionLogs;
using Newtonsoft.Json.Linq;
using Volo.Abp;
using Volo.Abp.Uow;

namespace Magicodes.Pay.Volo.Abp.Tests.Callback
{
    public class TestPaymentCallbackErrorAction : IPaymentCallbackAction
    {
        /// <summary>
        /// 业务Key
        /// </summary>
        public string Key { get; set; } = "缴费支付异常测试";

        /// <summary>
        /// 执行回调
        /// </summary>
        /// <returns></returns>
        public Task Process(IUnitOfWorkManager unitOfWork, TransactionLog transactionLog)
        {
            throw new BusinessException("支付报错！");
        }
    }
}

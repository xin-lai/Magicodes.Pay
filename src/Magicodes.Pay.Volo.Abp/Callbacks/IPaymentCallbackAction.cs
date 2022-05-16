using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Magicodes.Pay.Volo.Abp.TransactionLogs;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Uow;

namespace Magicodes.Pay.Volo.Abp.Callbacks
{
    /// <summary>
    /// 回调逻辑
    /// </summary>
    public interface IPaymentCallbackAction: ITransientDependency
    {
        /// <summary>
        /// 业务Key
        /// </summary>
        string Key { get; set; }

        /// <summary>
        /// 执行回调
        /// </summary>
        /// <returns></returns>
        Task Process(IUnitOfWorkManager unitOfWork, TransactionLog transactionLog);
    }
}

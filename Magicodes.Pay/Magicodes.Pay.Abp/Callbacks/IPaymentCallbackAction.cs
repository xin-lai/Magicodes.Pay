using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Abp.Dependency;
using Abp.Domain.Uow;
using Magicodes.Pay.Abp.TransactionLogs;

namespace Magicodes.Pay.Abp.Callbacks
{
    public interface IPaymentCallbackAction
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

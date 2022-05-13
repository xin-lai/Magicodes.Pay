using System.Threading.Tasks;
using Abp.Dependency;
using Abp.Domain.Repositories;

namespace Magicodes.Pay.Volo.Abp.TransactionLogs
{
    /// <summary>
    /// 交易日志仓储
    /// </summary>
    public class TransactionLogStore : ITransientDependency, ITransactionLogStore
    {
        private readonly IRepository<TransactionLog, long> _transactionLogRepository;

        public TransactionLogStore(IRepository<TransactionLog, long> transactionLogRepository)
        {
            _transactionLogRepository = transactionLogRepository;
        }

        public virtual Task SaveAsync(TransactionLog transactionLog)
        {
            return _transactionLogRepository.InsertAsync(TransactionLog.CreateTransactionLog(transactionLog));
        }

    }
}

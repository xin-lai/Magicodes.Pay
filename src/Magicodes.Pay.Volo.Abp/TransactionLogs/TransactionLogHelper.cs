// ======================================================================
//   
//           Copyright (C) 2018-2020 湖南心莱信息科技有限公司    
//           All rights reserved
//   
//           filename : TransactionLogHelper.cs
//           description :
//   
//           created by 雪雁 at  2018-08-06 14:21
//           Mail: wenqiang.li@xin-lai.com
//           QQ群：85318032（技术交流）
//           Blog：http://www.cnblogs.com/codelove/
//           GitHub：https://github.com/xin-lai
//           Home：http://xin-lai.com
//   
// ======================================================================

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System;
using System.Threading.Tasks;
using System.Transactions;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Settings;
using Volo.Abp.Timing;
using Volo.Abp.Uow;
using Volo.Abp.Users;

namespace Magicodes.Pay.Volo.Abp.TransactionLogs
{
    /// <summary>
    /// 交易日志辅助类
    /// </summary>
    public class TransactionLogHelper : ITransientDependency
    {
        private readonly ITransactionLogProvider _transactionLogProvider;

        private readonly ITransactionLogStore _transactionLogStore;

        private readonly IUnitOfWorkManager _unitOfWorkManager;

        private readonly IRepository<TransactionLog, long> _transactionLogRepository;
        private readonly ISettingProvider settingProvider;
        private readonly ICurrentUser currentUser;
        private readonly Clock clock;

        //private readonly ICurrentTenant currentTenant;

        public TransactionLogHelper(
            ITransactionLogProvider transactionLogProvider
            , IUnitOfWorkManager unitOfWorkManager
            , ITransactionLogStore transactionLogStore
            , IRepository<TransactionLog, long> transactionLogRepository
            , ISettingProvider settingProvider
            , ICurrentUser currentUser
            , Clock clock
            //, ICurrentTenant currentTenant
            , ILogger<TransactionLogHelper> logger)
        {
            _transactionLogProvider = transactionLogProvider;
            _unitOfWorkManager = unitOfWorkManager;
            _transactionLogStore = transactionLogStore;
            _transactionLogRepository = transactionLogRepository;
            this.settingProvider = settingProvider;
            this.currentUser = currentUser;
            this.clock = clock;
            //this.currentTenant = currentTenant;
            Logger = NullLogger<TransactionLogHelper>.Instance;
        }

        public ILogger<TransactionLogHelper> Logger { get; set; }

        /// <summary>
        /// 根据交易单号获取自定义数据
        /// </summary>
        /// <param name="outTradeNo"></param>
        /// <returns></returns>
        public string GetCustomDataByOutTradeNo(string outTradeNo) => GetTransactionLogByOutTradeNo(outTradeNo)?.CustomData;

        /// <summary>
        /// 根据交易单号获取交易日志信息
        /// </summary>
        /// <param name="outTradeNo"></param>
        /// <returns></returns>
        public async Task<TransactionLog> GetTransactionLogByOutTradeNo(string outTradeNo)
        {
            return await _transactionLogRepository.FirstOrDefaultAsync(p => p.OutTradeNo == outTradeNo);
        }

        /// <summary>
        ///     创建交易日志
        /// </summary>
        /// <param name="transactionInfo"></param>
        /// <param name="symbol">货币符号</param>
        /// <returns></returns>
        public TransactionLog CreateTransactionLog(TransactionInfo transactionInfo)
        {
            var log = new TransactionLog
            {
                TenantId = currentUser.TenantId,
                CreatorId = currentUser.Id,
                Amount = transactionInfo.Amount,
                Name = transactionInfo.Subject,
                CustomData = transactionInfo.CustomData,
                OutTradeNo = transactionInfo.OutTradeNo,
                PayChannel = transactionInfo.PayChannel,
                TransactionState = transactionInfo.TransactionState,
                TransactionId = transactionInfo.TransactionId,
                PayTime = transactionInfo.PayTime
            };
            try
            {
                _transactionLogProvider.Fill(log, transactionInfo.Exception);
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
            }

            return log;
        }

        /// <summary>
        ///     提交交易日志
        /// </summary>
        /// <param name="transactionLog"></param>
        /// <returns></returns>
        public async Task SaveAsync(TransactionLog transactionLog)
        {
            using (var uow = _unitOfWorkManager.Begin(requiresNew: true, isTransactional: false))
            {
                await _transactionLogStore.SaveAsync(transactionLog);
                await uow.CompleteAsync();
            }
        }

        /// <summary>
        /// 执行业务逻辑并更新交易日志
        /// </summary>
        /// <param name="outTradeNo">商户系统的订单号</param>
        /// <param name="transactionId">微信支付订单号</param>
        /// <param name="action">业务逻辑</param>
        /// <returns></returns>
        public async Task UpdateAsync(string outTradeNo, string transactionId, Func<IUnitOfWorkManager, TransactionLog, Task> action)
        {
            Exception exception = null;
            using (var uow = _unitOfWorkManager.Begin(requiresNew: true, isTransactional: false))
            {

                var logInfo = await _transactionLogRepository.FirstOrDefaultAsync(p => p.OutTradeNo == outTradeNo);
                if (logInfo == null)
                {
                    Logger.LogError("交易订单号为 " + outTradeNo + " 不存在！");
                    return;
                }

                if (logInfo.TransactionState == TransactionStates.Success && logInfo.PayTime.HasValue)
                {
                    Logger.LogError($"outTradeNo: {outTradeNo} ，transactionId：{transactionId} 的订单已经完成了交易，请不要重新发起操作！");
                    return;
                }

                try
                {
                    logInfo.PayTime = clock.Now;
                    logInfo.TransactionId = transactionId;
                    await action(_unitOfWorkManager, logInfo);
                    logInfo.TransactionState = TransactionStates.Success;
                    await uow.CompleteAsync();
                }
                catch (Exception ex)
                {
                    logInfo.TransactionState = TransactionStates.PayError;
                    logInfo.TransactionId = transactionId;
                    logInfo.PayTime = clock.Now;
                    logInfo.Exception = ex.InnerException != null ? ex.InnerException.ToString().TruncateWithPostfix(2000) : ex.ToString().TruncateWithPostfix(2000);
                    await uow.CompleteAsync();
                    exception = ex;
                }
            }
            if (exception != null)
                throw exception.InnerException ?? exception;
        }
    }
}
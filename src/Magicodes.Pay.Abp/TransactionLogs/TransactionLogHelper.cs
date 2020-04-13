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

using System;
using System.Threading.Tasks;
using System.Transactions;
using Abp.Configuration;
using Abp.Dependency;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Extensions;
using Abp.Runtime.Session;
using Abp.Timing;
using Castle.Core.Logging;

namespace Magicodes.Pay.Abp.TransactionLogs
{
    /// <summary>
    /// 交易日志辅助类
    /// </summary>
    public class TransactionLogHelper : ITransientDependency
    {
        private readonly ITransactionLogProvider _transactionLogProvider;

        private readonly ITransactionLogStore _transactionLogStore;

        private readonly IUnitOfWorkManager _unitOfWorkManager;

        private readonly ISettingManager _settingManager;

        private readonly IRepository<TransactionLog, long> _transactionLogRepository;

        public TransactionLogHelper(
            ITransactionLogProvider transactionLogProvider
            , IUnitOfWorkManager unitOfWorkManager
            , ITransactionLogStore transactionLogStore
            , ISettingManager settingManager
            , IRepository<TransactionLog, long> transactionLogRepository)
        {
            _transactionLogProvider = transactionLogProvider;
            _unitOfWorkManager = unitOfWorkManager;
            _transactionLogStore = transactionLogStore;
            _settingManager = settingManager;
            _transactionLogRepository = transactionLogRepository;
            AbpSession = NullAbpSession.Instance;
            Logger = NullLogger.Instance;
        }

        public ILogger Logger { get; set; }
        public IAbpSession AbpSession { get; set; }

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
        public TransactionLog GetTransactionLogByOutTradeNo(string outTradeNo)
        {
            _unitOfWorkManager.Current.SetTenantId(AbpSession.TenantId);
            return _transactionLogRepository.FirstOrDefault(p => p.OutTradeNo == outTradeNo);
        }

        /// <summary>
        ///     创建交易日志
        /// </summary>
        /// <param name="transactionInfo"></param>
        /// <param name="symbol">货币符号</param>
        /// <returns></returns>
        public TransactionLog CreateTransactionLog(TransactionInfo transactionInfo, string symbol = "CNY")
        {
            var cultureName = _settingManager.GetSettingValueAsync("Abp.Localization.DefaultLanguageName").Result;
            var log = new TransactionLog
            {
                //TenantId = AbpSession.TenantId,
                CreatorUserId = AbpSession.UserId,
                Currency = new Currency(transactionInfo.Amount, symbol),
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
                Logger.Warn(ex.ToString(), ex);
            }

            return log;
        }

        /// <summary>
        ///     保存交易日志
        /// </summary>
        /// <param name="transactionLog"></param>
        public void Save(TransactionLog transactionLog)
        {
            using (var uow = _unitOfWorkManager.Begin(TransactionScopeOption.Suppress))
            {
                _transactionLogStore.Save(transactionLog);
                uow.Complete();
            }
        }

        /// <summary>
        ///     提交交易日志
        /// </summary>
        /// <param name="transactionLog"></param>
        /// <returns></returns>
        public async Task SaveAsync(TransactionLog transactionLog)
        {
            using (var uow = _unitOfWorkManager.Begin(TransactionScopeOption.Suppress))
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
            using (var uow = _unitOfWorkManager.Begin(TransactionScopeOption.Suppress))
            {

                var logInfo = await _transactionLogRepository.FirstOrDefaultAsync(p => p.OutTradeNo == outTradeNo);
                if (logInfo == null)
                {
                    Logger.Error("交易订单号为 " + outTradeNo + " 不存在！");
                    return;
                }

                if (logInfo.TransactionState == TransactionStates.Success && logInfo.PayTime.HasValue)
                {
                    Logger.Error($"outTradeNo: {outTradeNo} ，transactionId：{transactionId} 的订单已经完成了交易，请不要重新发起操作！");
                    return;
                }

                try
                {
                    await action(_unitOfWorkManager, logInfo);
                    logInfo.TransactionState = TransactionStates.Success;
                    logInfo.PayTime = Clock.Now;
                    logInfo.TransactionId = transactionId;
                    await uow.CompleteAsync();
                }
                catch (Exception ex)
                {
                    logInfo.TransactionState = TransactionStates.PayError;
                    logInfo.PayTime = Clock.Now;
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
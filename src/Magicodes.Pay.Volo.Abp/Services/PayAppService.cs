// ======================================================================
// 
//           Copyright (C) 2019-2030 湖南心莱信息科技有限公司
//           All rights reserved
// 
//           filename : PayAppService.cs
//           description :
// 
//           created by 雪雁 at  -- 
//           文档官网：https://docs.xin-lai.com
//           公众号教程：麦扣聊技术
//           QQ群：85318032（编程交流）
//           Blog：http://www.cnblogs.com/codelove/
// 
// ======================================================================

using System;
using System.Threading.Tasks;
using Magicodes.Pay.Volo.Abp.TransactionLogs;
using Magicodes.Pay.Volo.Abp.Services.Dto;
using Microsoft.Extensions.Logging;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Json;
using Volo.Abp;

namespace Magicodes.Pay.Volo.Abp.Services
{
    /// <summary>
    ///  统一支付服务
    /// </summary>
    public class PayAppService : IPayAppService, ITransientDependency
    {
        private readonly IPaymentManager _paymentManager;
        private readonly TransactionLogHelper _transactionLogHelper;
        private readonly Logger<PayAppService> logger;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="paymentManager"></param>
        /// <param name="transactionLogHelper"></param>
        public PayAppService(IPaymentManager paymentManager, TransactionLogHelper transactionLogHelper,Logger<PayAppService> logger)
        {
            _paymentManager = paymentManager;
            _transactionLogHelper = transactionLogHelper;
            this.logger = logger;
        }

        /// <summary>
        /// 统一支付（支持各个渠道）
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public virtual async Task<object> Pay(PayInputBase input)
        {
            logger.LogDebug("准备发起支付：" + input);
            Exception exception = null;
            object output = null;
            if (input.OutTradeNo.IsNullOrWhiteSpace())
            {
                input.OutTradeNo = GenerateOutTradeNo();
            }

            try
            {
                var service = await _paymentManager.GetPayService(input.PayChannel);
                if (service == null) throw new BusinessException($"支付渠道 {input.PayChannel} 不存在，请确认是否已注册或已添加引用！");
                if (string.IsNullOrWhiteSpace(input.OutTradeNo))
                {
                    input.OutTradeNo = $"{DateTime.Now:yyyyMMddHHmmssfff}";
                }
                output = await service.ToPay(input);
            }
            catch (Exception ex)
            {
                exception = ex;
            }
            await CreateToPayTransactionInfo(input, exception);
            if (exception == null) return output;

            logger.LogError("支付失败！", exception);
            throw new BusinessException("支付异常，请联系客服人员或稍后再试！");
        }

        /// <summary>
        /// 创建交易日志
        /// </summary>
        /// <returns></returns>
        private async Task<TransactionLog> CreateToPayTransactionInfo(PayInputBase input, Exception exception = null)
        {
            var transactionInfo = new TransactionInfo()
            {
                Amount = input.TotalAmount,
                CustomData = input.CustomData,
                OutTradeNo = input.OutTradeNo ?? GenerateOutTradeNo(),
                PayChannel = input.PayChannel,
                Subject = input.Subject,
                TransactionState = TransactionStates.NotPay,
                //TransactionId = "",
                Exception = exception
            };
            TransactionLog transactionLog = null;
            transactionLog = _transactionLogHelper.CreateTransactionLog(transactionInfo);
            await _transactionLogHelper.SaveAsync(transactionLog);
            return transactionLog;
        }

        /// <summary>
        /// 生成交易单号
        /// </summary>
        /// <returns></returns>
        private string GenerateOutTradeNo()
        {
            var code = RandomHelper.GetRandom(1000, 9999);
            return $"M{Clock.Now:yyyyMMddHHmmssfff}{code}";
        }

    }
}
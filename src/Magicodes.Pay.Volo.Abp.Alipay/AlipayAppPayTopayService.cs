using System;
using System.Threading.Tasks;
using Magicodes.Pay.Alipay;
using Magicodes.Pay.Alipay.Dto;
using Magicodes.Pay.Volo.Abp.Services;
using Magicodes.Pay.Volo.Abp.Services.Dto;
using Magicodes.Pay.Volo.Abp.TransactionLogs;
using Volo.Abp;

namespace Magicodes.Pay.Abp.Alipay
{
    /// <summary>
    /// 
    /// </summary>
    public class AlipayAppPayTopayService : ToPayServiceBase
    {
        private readonly IAlipayAppService alipayAppService;

        /// <summary>
        /// 
        /// </summary>
        public AlipayAppPayTopayService(IAlipayAppService alipayAppService) : base()
        {
            this.alipayAppService = alipayAppService;
        }

        /// <summary>
        /// 支付渠道
        /// </summary>
        public override PayChannels PayChannel => PayChannels.AliAppPay;

        /// <summary>
        /// 支付
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public override async Task<object> ToPay(PayInputBase input)
        {
            var appPayInput = new AppPayInput()
            {
                Body = input.Body,
                Subject = input.Subject,
                TradeNo = input.OutTradeNo,
                PassbackParams = input.CustomData,
                TotalAmount = input.TotalAmount
            };
            try
            {
                var appPayOutput = await alipayAppService.AppPay(appPayInput);
                return appPayOutput.Response.Body;
            }
            catch (Exception ex)
            {
                throw new BusinessException(message: ex.Message);
            }
        }
    }
}

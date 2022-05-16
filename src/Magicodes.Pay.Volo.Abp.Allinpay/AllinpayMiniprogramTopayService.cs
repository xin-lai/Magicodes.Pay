using System;
using System.Threading.Tasks;
using Magicodes.Pay.Allinpay;
using Magicodes.Pay.Allinpay.Dto;
using Magicodes.Pay.Volo.Abp.Services;
using Magicodes.Pay.Volo.Abp.Services.Dto;
using Magicodes.Pay.Volo.Abp.TransactionLogs;
using Volo.Abp;

namespace Magicodes.Pay.Volo.Abp.Allinpay
{
    /// <summary>
    /// 
    /// </summary>
    public class AllinpayMiniprogramTopayService : ToPayServiceBase
    {
        private readonly IAllinpayAppService allinpayAppService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="iocResolver"></param>
        public AllinpayMiniprogramTopayService(IAllinpayAppService allinpayAppService) : base()
        {
            this.allinpayAppService = allinpayAppService;
        }

        /// <summary>
        /// 支付渠道
        /// </summary>
        public override PayChannels PayChannel => PayChannels.AllinWeChatMiniPay;

        /// <summary>
        /// 支付
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public override async Task<object> ToPay(PayInputBase input)
        {
            var appPayInput = new WeChatMiniPayInput()
            {
                Body = input.Body,
                OrderNumber = input.OutTradeNo,
                Amount = (int)(input.TotalAmount * 100),
                OpenId = input.OpenId,
                Remark = input.Key
            };
            try
            {
                var appPayOutput = await allinpayAppService.WeChatMiniPay(appPayInput);
                return appPayOutput.Response;
            }
            catch (Exception ex)
            {
                throw new BusinessException(ex.Message);
            }
        }
    }
}

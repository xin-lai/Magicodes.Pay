using Magicodes.Pay.Allinpay;
using Magicodes.Pay.Allinpay.Dto;
using Magicodes.Pay.Volo.Abp.Services;
using Magicodes.Pay.Volo.Abp.Services.Dto;
using Magicodes.Pay.Volo.Abp.TransactionLogs;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.DependencyInjection;

namespace Magicodes.Pay.Volo.Abp.Allinpay
{
    /// <summary>
    /// 
    /// </summary>
    public class AllinpayJsApiTopayService : ToPayServiceBase
    {
        private readonly IAllinpayAppService allinpayAppService;

        /// <summary>
        /// 
        /// </summary>
        public AllinpayJsApiTopayService(IAllinpayAppService allinpayAppService) : base()
        {
            this.allinpayAppService = allinpayAppService;
        }

        /// <summary>
        /// 支付渠道
        /// </summary>
        public override PayChannels PayChannel => PayChannels.AllinJsApiPay;

        /// <summary>
        /// 支付
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public override async Task<object> ToPay(PayInputBase input)
        {
            var appPayInput = new JsApiPayInput()
            {
                Body = input.Body,
                OrderNumber = input.OutTradeNo,
                Amount = (int)(input.TotalAmount * 100),
                OpenId = input.OpenId,
                Remark = input.Key
            };
            try
            {
                var appPayOutput = await allinpayAppService.WeChatJsApiPay(appPayInput);
                return appPayOutput.Response;
            }
            catch (Exception ex)
            {
                throw new BusinessException(message:ex.Message);
            }
        }

    }
}

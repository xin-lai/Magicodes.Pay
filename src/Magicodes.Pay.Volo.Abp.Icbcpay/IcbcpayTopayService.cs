using Magicodes.Pay.Icbc;
using Magicodes.Pay.Icbc.Dto;
using Magicodes.Pay.Volo.Abp.Services;
using Magicodes.Pay.Volo.Abp.Services.Dto;
using Magicodes.Pay.Volo.Abp.TransactionLogs;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.DependencyInjection;

namespace Magicodes.Pay.Volo.Abp.Icbcpay
{
    /// <summary>
    /// 
    /// </summary>
    public class IcbcpayTopayService : ToPayServiceBase
    {
        private readonly IIcbcpayAppService IcbcpayAppService;

        /// <summary>
        /// 
        /// </summary>
        public IcbcpayTopayService(IIcbcpayAppService IcbcpayAppService) : base()
        {
            this.IcbcpayAppService = IcbcpayAppService;
        }

        /// <summary>
        /// 支付渠道
        /// </summary>
        public override PayChannels PayChannel => PayChannels.IcbcPay;

        /// <summary>
        /// 支付
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public override async Task<object> ToPay(PayInputBase input)
        {
            var appPayInput = new PayInput()
            {
                Body = input.Body,
                OrderNumber = input.OutTradeNo,
                Amount = (int)(input.TotalAmount * 100),
                OpenId = input.OpenId,
                Remark = input.Key,
                ValidTime = "1200"
            };
            try
            {
                var appPayOutput = await IcbcpayAppService.B2cAggregatedPay(appPayInput);
                return appPayOutput.Response;
            }
            catch (Exception ex)
            {
                throw new BusinessException(message:ex.Message);
            }
        }

    }
}

using Magicodes.Pay.Volo.Abp.Services;
using Magicodes.Pay.Volo.Abp.Services.Dto;
using Magicodes.Pay.Volo.Abp.TransactionLogs;
using Magicodes.Pay.Wxpay.Pay;
using Magicodes.Pay.Wxpay.Pay.Dto;
using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;
using Volo.Abp;

namespace Magicodes.Pay.Volo.Abp.Wxpay
{
    /// <summary>
    ///  微信APP支付
    /// </summary>
    public class WeChatAppPayTopayService : ToPayServiceBase
    {
        private readonly WeChatPayApi weChatPayApi;
        private readonly IHttpContextAccessor httpContextAccessor;

        public WeChatAppPayTopayService(WeChatPayApi weChatPayApi, IHttpContextAccessor httpContextAccessor)
        {
            this.weChatPayApi = weChatPayApi;
            this.httpContextAccessor = httpContextAccessor;
        }

        /// <summary>
        /// 支付渠道
        /// </summary>
        public override PayChannels PayChannel => PayChannels.WeChatAppPay;

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
                OutTradeNo = input.OutTradeNo,
                Attach = input.Key,
                TotalFee = input.TotalAmount,
                SpbillCreateIp = httpContextAccessor?.HttpContext?.Connection.RemoteIpAddress?.ToString()
            };
            try
            {
                var appPayOutput = weChatPayApi.AppPay(appPayInput);
                return await Task.FromResult(appPayOutput);
            }
            catch (Exception ex)
            {
                throw new BusinessException(message: ex.Message);
            }
        }
    }
}

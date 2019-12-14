using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Abp.Auditing;
using Abp.Dependency;
using Abp.UI;
using Magicodes.Pay.Abp.Services;
using Magicodes.Pay.Abp.Services.Dto;
using Magicodes.Pay.Abp.TransactionLogs;
using Magicodes.Pay.Wxpay.Pay;
using Magicodes.Pay.Wxpay.Pay.Dto;

namespace Magicodes.Pay.Abp.Wxpay
{
    /// <summary>
    /// 
    /// </summary>
    public class WeChatMiniprogramTopayService : ToPayServiceBase
    {
        private readonly IClientInfoProvider _clientInfoProvider;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="iocResolver"></param>
        /// <param name="clientInfoProvider"></param>
        public WeChatMiniprogramTopayService(IIocResolver iocResolver, IClientInfoProvider clientInfoProvider) : base(iocResolver)
        {
            _clientInfoProvider = clientInfoProvider;
        }

        /// <summary>
        /// 支付渠道
        /// </summary>
        public override PayChannels PayChannel => PayChannels.WeChatMiniProgram;

        /// <summary>
        /// 支付
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public override async Task<object> ToPay(PayInputBase input)
        {
            var weChatPayApi = IocResolver.Resolve<WeChatPayApi>();
            if (weChatPayApi == null)
            {
                throw new UserFriendlyException("支付未开放，请联系管理员！");
            }
            var appPayInput = new MiniProgramPayInput()
            {
                Body = input.Body,
                OutTradeNo = input.OutTradeNo,
                Attach = input.Key,
                TotalFee = (int)input.TotalAmount*100,
                SpbillCreateIp = _clientInfoProvider?.ClientIpAddress,
                OpenId = input.OpenId,
            };
            try
            {
                var appPayOutput = weChatPayApi.MiniProgramPay(appPayInput);
                return await Task.FromResult(appPayOutput);
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }
    }
}

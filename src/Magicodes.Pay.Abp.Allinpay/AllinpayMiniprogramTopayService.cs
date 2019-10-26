using System;
using System.Threading.Tasks;
using Abp.Dependency;
using Abp.UI;
using Magicodes.Pay.Abp.Services;
using Magicodes.Pay.Abp.Services.Dto;
using Magicodes.Pay.Abp.TransactionLogs;
using Magicodes.Pay.Allinpay;
using Magicodes.Pay.Allinpay.Dto;

namespace Magicodes.Pay.Abp.Allinpay
{
    /// <summary>
    /// 
    /// </summary>
    public class AllinpayMiniprogramTopayService : ToPayServiceBase
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="iocResolver"></param>
        public AllinpayMiniprogramTopayService(IIocResolver iocResolver) : base(iocResolver)
        {
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
            var api = IocResolver.Resolve<IAllinpayAppService>();
            if (api == null)
            {
                throw new UserFriendlyException("支付未开放，请联系管理员！");
            }
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
                var appPayOutput = await api.WeChatMiniPay(appPayInput);
                return appPayOutput.Response;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }
    }
}

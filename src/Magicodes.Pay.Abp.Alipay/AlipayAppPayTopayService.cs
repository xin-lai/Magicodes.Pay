using System;
using System.Threading.Tasks;
using Abp.Dependency;
using Abp.UI;
using Magicodes.Pay.Abp.Services;
using Magicodes.Pay.Abp.Services.Dto;
using Magicodes.Pay.Abp.TransactionLogs;
using Magicodes.Pay.Alipay;
using Magicodes.Pay.Alipay.Dto;

namespace Magicodes.Pay.Abp.Alipay
{
    /// <summary>
    /// 
    /// </summary>
    public class AlipayAppPayTopayService : ToPayServiceBase
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="iocResolver"></param>
        public AlipayAppPayTopayService(IIocResolver iocResolver) : base(iocResolver)
        {
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
            var api = IocResolver.Resolve<IAlipayAppService>();
            if (api == null)
            {
                throw new UserFriendlyException("支付未开放，请联系管理员！");
            }
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
                var appPayOutput = await api.AppPay(appPayInput);
                return appPayOutput.Response.Body;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }
    }
}

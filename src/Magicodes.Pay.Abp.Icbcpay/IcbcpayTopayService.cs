using Abp.Dependency;
using Abp.UI;
using Magicodes.Pay.Abp.Services;
using Magicodes.Pay.Abp.Services.Dto;
using Magicodes.Pay.Abp.TransactionLogs;
using Magicodes.Pay.Icbc;
using Magicodes.Pay.Icbc.Dto;
using System;
using System.Threading.Tasks;

namespace Magicodes.Pay.Abp.Icbcpay
{
    /// <summary>
    /// 
    /// </summary>
    public class IcbcpayTopayService : ToPayServiceBase
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="iocResolver"></param>
        public IcbcpayTopayService(IIocResolver iocResolver) : base(iocResolver)
        {
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
            var api = IocResolver.Resolve<IIcbcpayAppService>();
            if (api == null)
            {
                throw new UserFriendlyException("支付未开放，请联系管理员！");
            }
            var appPayInput = new PayInput()
            {
                Body = input.Body,
                OrderNumber = input.OutTradeNo,
                Amount = (int)(input.TotalAmount * 100),
                OpenId = input.OpenId,
                Remark = input.Key
            };
            try
            {
                var appPayOutput = await api.B2cAggregatedPay(appPayInput);
                return appPayOutput;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }

    }
}

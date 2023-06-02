using Magicodes.Pay.Icbc.Dto;
using Magicodes.Pay.Icbcpay;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Magicodes.Pay.Icbc
{
    public interface IIcbcpayAppService
    {
        /// <summary>
        /// b2c聚合支付接口
        /// </summary>
        Task<B2cAggregatedPayOutput> B2cAggregatedPay(PayInput input);

        IcbcpayOutput PayNotifyHandler(Dictionary<string, string> dic);

    }
}

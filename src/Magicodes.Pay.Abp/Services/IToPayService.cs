using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Magicodes.Pay.Abp.Services.Dto;
using Magicodes.Pay.Abp.TransactionLogs;

namespace Magicodes.Pay.Abp.Services
{
    /// <summary>
    /// 支付服务接口实现
    /// </summary>
    public interface IToPayService
    {
        /// <summary>
        /// 支付渠道
        /// </summary>
        PayChannels PayChannel { get; }

        /// <summary>
        /// 支付
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<object> ToPay(PayInputBase input);
    }
}

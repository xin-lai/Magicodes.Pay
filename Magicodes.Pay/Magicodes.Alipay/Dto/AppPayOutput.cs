using Alipay.AopSdk.Core.Response;
using System;
using System.Collections.Generic;
using System.Text;

namespace Magicodes.Alipay.Dto
{
    /// <summary>
    /// APP支付输出
    /// </summary>
    public class AppPayOutput
    {
        /// <summary>
        /// 支付结果
        /// </summary>
        public AlipayTradeAppPayResponse Response { get; set; }
    }
}

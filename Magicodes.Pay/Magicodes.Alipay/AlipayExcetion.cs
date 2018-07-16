using System;
using System.Collections.Generic;
using System.Text;

namespace Magicodes.Alipay
{
    public class AlipayExcetion : Exception
    {
        public AlipayExcetion(string 支付宝支付请求参数错误请检查)
        {
        }

        public AlipayExcetion(string message, Exception exception) : base(message)
        {
        }
    }
}

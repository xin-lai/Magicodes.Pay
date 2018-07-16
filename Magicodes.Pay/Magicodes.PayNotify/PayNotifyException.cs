using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Magicodes.PayNotify
{
    /// <summary>
    /// 支付回调异常
    /// </summary>
    public class PayNotifyException : Exception
    {
        public PayNotifyException()
        {
        }

        public PayNotifyException(string message) : base(message)
        {
        }

        public PayNotifyException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected PayNotifyException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}

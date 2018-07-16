using System;
using System.Runtime.Serialization;

namespace Magicodes.Pay.WeChat
{
    [Serializable]
    internal class PayException : Exception
    {
        public PayException()
        {
        }

        public PayException(string message) : base(message)
        {
        }

        public PayException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected PayException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
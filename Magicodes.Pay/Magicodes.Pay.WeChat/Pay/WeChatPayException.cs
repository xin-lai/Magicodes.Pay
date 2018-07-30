using System;
using System.Runtime.Serialization;

namespace Magicodes.Pay.WeChat
{
    [Serializable]
    internal class WeChatPayException : Exception
    {
        public WeChatPayException()
        {
        }

        public WeChatPayException(string message) : base(message)
        {
        }

        public WeChatPayException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected WeChatPayException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
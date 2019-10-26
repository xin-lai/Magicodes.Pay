// ======================================================================
//   
//           Copyright (C) 2018-2020 湖南心莱信息科技有限公司    
//           All rights reserved
//   
//           filename : PayNotifyException.cs
//           description :
//   
//           created by 雪雁 at  2018-07-16 15:46
//           Mail: wenqiang.li@xin-lai.com
//           QQ群：85318032（技术交流）
//           Blog：http://www.cnblogs.com/codelove/
//           GitHub：https://github.com/xin-lai
//           Home：http://xin-lai.com
//   
// ======================================================================

using System;
using System.Runtime.Serialization;

namespace Magicodes.Pay.Notify
{
    /// <summary>
    ///     支付回调异常
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
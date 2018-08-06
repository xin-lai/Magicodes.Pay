// ======================================================================
//   
//           Copyright (C) 2018-2020 湖南心莱信息科技有限公司    
//           All rights reserved
//   
//           filename : WeChatPayException.cs
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
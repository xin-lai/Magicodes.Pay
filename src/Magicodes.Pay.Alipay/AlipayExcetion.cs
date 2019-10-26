// ======================================================================
//   
//           Copyright (C) 2018-2020 湖南心莱信息科技有限公司    
//           All rights reserved
//   
//           filename : AlipayExcetion.cs
//           description :
//   
//           created by 雪雁 at  2018-07-16 15:59
//           Mail: wenqiang.li@xin-lai.com
//           QQ群：85318032（技术交流）
//           Blog：http://www.cnblogs.com/codelove/
//           GitHub：https://github.com/xin-lai
//           Home：http://xin-lai.com
//   
// ======================================================================

using System;

namespace Magicodes.Pay.Alipay
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
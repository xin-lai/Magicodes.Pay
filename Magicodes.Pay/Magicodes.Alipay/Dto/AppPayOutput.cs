// ======================================================================
//   
//           Copyright (C) 2018-2020 湖南心莱信息科技有限公司    
//           All rights reserved
//   
//           filename : AppPayOutput.cs
//           description :
//   
//           created by 雪雁 at  2018-07-16 15:42
//           Mail: wenqiang.li@xin-lai.com
//           QQ群：85318032（技术交流）
//           Blog：http://www.cnblogs.com/codelove/
//           GitHub：https://github.com/xin-lai
//           Home：http://xin-lai.com
//   
// ======================================================================

using Alipay.AopSdk.Core.Response;

namespace Magicodes.Alipay.Dto
{
    /// <summary>
    ///     APP支付输出
    /// </summary>
    public class AppPayOutput
    {
        /// <summary>
        ///     支付结果
        /// </summary>
        public AlipayTradeAppPayResponse Response { get; set; }
    }
}
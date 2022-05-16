// ======================================================================
//   
//           Copyright (C) 2018-2020 湖南心莱信息科技有限公司    
//           All rights reserved
//   
//           filename : PayNotifyInput.cs
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

using Microsoft.AspNetCore.Http;

namespace Magicodes.Pay.Notify.Models
{
    public class PayNotifyInput
    {
        /// <summary>
        ///     租户Id（选填）
        /// </summary>
        public string TenantId { get; set; }

        /// <summary>
        ///     提供程序简称，比如wechat、alipay
        /// </summary>
        public string Provider { get; set; }

        /// <summary>
        ///     当前请求
        /// </summary>
        public HttpRequest Request { get; set; }
    }
}
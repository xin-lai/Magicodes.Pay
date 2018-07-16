using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Http;

namespace Magicodes.PayNotify.Models
{
    public class PayNotifyInput
    {
        /// <summary>
        /// 租户Id（选填）
        /// </summary>
        public int? TenantId { get; set; }

        /// <summary>
        /// 提供程序简称，比如wechat、alipay
        /// </summary>
        public string Provider { get; set; }

        /// <summary>
        /// 当前请求
        /// </summary>
        public HttpRequest Request { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Magicodes.PayNotify.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Magicodes.PayNotify
{
    /// <summary>
    /// 支付回调控制器
    /// </summary>
    [AllowAnonymous]
    [Route("PayNotify")]
    public class PayNotifyController : Controller
    {
        internal static Action<string, string> LoggerAction = (tag, log) => { };

        internal static Func<PayNotifyInput, Task<string>> PayNotifyFunc = (req) =>
         {
             throw new PayNotifyException("请对接支付回调处理逻辑！");
         };

        /// <summary>
        /// 支付回调
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="provider">提供程序，比如WeChat、Alipay</param>
        /// <returns></returns>
        [HttpPost("{tenantId?}/{provider}")]
        public async Task<IActionResult> PayNotify(int? tenantId, string provider)
        {
            if (string.IsNullOrWhiteSpace(provider))
            {
                throw new ArgumentException("请传递提供程序！", nameof(provider));
            }

            LoggerAction("Debug", "正在处理支付信息");
            var input = new PayNotifyInput()
            {
                TenantId = tenantId,
                Provider = provider,
                Request = Request
            };
            var result = await Task.Run(() =>
            {
                try
                {
                    return PayNotifyFunc(input);
                }
                catch (Exception ex)
                {
                    LoggerAction("Error", "调用回调处理逻辑时出错：" + ex?.ToString());
                    throw new PayNotifyException("调用回调处理逻辑时出错!");
                }
            });
            LoggerAction("Debug", "回调处理结果：" + result);
            return Content(result);
        }
    }
}

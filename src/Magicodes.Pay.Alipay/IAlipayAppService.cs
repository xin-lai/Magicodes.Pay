// ======================================================================
//   
//           Copyright (C) 2018-2020 湖南心莱信息科技有限公司    
//           All rights reserved
//   
//           filename : IAlipayAppService.cs
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

using System.Collections.Generic;
using System.Threading.Tasks;
using Magicodes.Pay.Alipay.Dto;

namespace Magicodes.Pay.Alipay
{
    /// <summary>
    ///     支付宝支付
    /// </summary>
    public interface IAlipayAppService
    {
        /// <summary>
        ///     APP支付
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<AppPayOutput> AppPay(AppPayInput input);

        /// <summary>
        ///     WAP支付
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<WapPayOutput> WapPay(WapPayInput input);

        /// <summary>
        ///     支付回调
        /// </summary>
        /// <param name="dic"></param>
        /// <returns></returns>
        bool PayNotifyHandler(Dictionary<string, string> dic);
    }
}
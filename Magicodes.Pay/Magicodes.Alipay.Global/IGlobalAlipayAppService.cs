// ======================================================================
//   
//           Copyright (C) 2018-2020 湖南心莱信息科技有限公司    
//           All rights reserved
//   
//           filename : IGlobalAlipayAppService.cs
//           description :
//   
//           created by 雪雁 at  2018-11-23 9:27
//           Mail: wenqiang.li@xin-lai.com
//           QQ群：85318032（技术交流）
//           Blog：http://www.cnblogs.com/codelove/
//           GitHub：https://github.com/xin-lai
//           Home：http://xin-lai.com
//   
// ======================================================================

using Magicodes.Alipay.Global.Dto;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Magicodes.Alipay.Global
{
    /// <summary>
    ///     国际支付宝支付
    /// </summary>
    public interface IGlobalAlipayAppService
    {
        /// <summary>
        ///     网站支付
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<PayOutput> Pay(PayInput input);

        /// <summary>
        ///     支付回调
        /// </summary>
        /// <param name="dic"></param>
        /// <returns></returns>
        bool PayNotifyHandler(Dictionary<string, string> dic);
    }
}
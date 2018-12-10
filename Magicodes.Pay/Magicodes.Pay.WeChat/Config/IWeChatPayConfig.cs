// ======================================================================
//   
//           Copyright (C) 2018-2020 湖南心莱信息科技有限公司    
//           All rights reserved
//   
//           filename : IWeChatPayConfig.cs
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

namespace Magicodes.Pay.WeChat.Config
{
    /// <summary>
    ///     微信支付配置信息
    /// </summary>
    public interface IWeChatPayConfig
    {
        /// <summary>
        ///     APPId
        /// </summary>
        string PayAppId { get; set; }

        /// <summary>
        ///     商户Id
        /// </summary>
        string MchId { get; set; }

        /// <summary>
        ///     支付回调路径
        /// </summary>
        string PayNotifyUrl { get; set; }

        /// <summary>
        ///     支付密钥
        /// </summary>
        string TenPayKey { get; set; }
    }
}
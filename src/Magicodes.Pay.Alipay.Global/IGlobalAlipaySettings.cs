// ======================================================================
//   
//           Copyright (C) 2018-2020 湖南心莱信息科技有限公司    
//           All rights reserved
//   
//           filename : IGlobalAlipaySettings.cs
//           description :
//   
//           created by 雪雁 at  2018-11-23 9:07
//           Mail: wenqiang.li@xin-lai.com
//           QQ群：85318032（技术交流）
//           Blog：http://www.cnblogs.com/codelove/
//           GitHub：https://github.com/xin-lai
//           Home：http://xin-lai.com
//   
// ======================================================================

using System.Collections.Generic;
using Magicodes.Pay.Alipay.Global.Dto;

namespace Magicodes.Pay.Alipay.Global
{
    /// <summary>
    ///     Defines the <see cref="IGlobalAlipaySettings" />
    /// </summary>
    public interface IGlobalAlipaySettings
    {
        /// <summary>
        ///     Gets or sets the Key
        ///     MD5密钥，安全检验码，由数字和字母组成的32位字符串，查看地址：https://b.alipay.com/order/pidAndKey.htm
        /// </summary>
        string Key { get; set; }

        /// <summary>
        ///     Gets or sets the Partner
        ///     合作商户uid(合作身份者ID，签约账号，以2088开头由16位纯数字组成的字符串，查看地址：https://b.alipay.com/order/pidAndKey.htm)
        /// </summary>
        string Partner { get; set; }

        /// <summary>
        ///     Gets or sets the Gatewayurl
        ///     支付宝网关
        /// </summary>
        string Gatewayurl { get; set; }

        /// <summary>
        ///     Gets or sets the SignType
        ///     签名方式（默认值：MD5）
        /// </summary>
        string SignType { get; set; }

        /// <summary>
        ///     Gets or sets the CharSet
        ///     编码格式（默认值：UTF-8）
        /// </summary>
        string CharSet { get; set; }

        /// <summary>
        ///     Gets or sets the Notify
        ///     服务器异步通知页面路径，需http://格式的完整路径，不能加?id=123这类自定义参数,必须外网可以正常访问
        /// </summary>
        string Notify { get; set; }

        /// <summary>
        ///     页面跳转同步通知页面路径，需http://格式的完整路径，不能加?id=123这类自定义参数，必须外网可以正常访问
        /// </summary>
        string ReturnUrl { get; set; }

        /// <summary>
        ///     结算币种
        /// </summary>
        string Currency { get; set; }

        /// <summary>
        /// 分账信息
        /// </summary>
        List<SplitFundSettingInfoDto> SplitFundInfo { get; set; }
    }
}
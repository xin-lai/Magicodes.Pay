using System;
using System.Collections.Generic;
using System.Text;

namespace Magicodes.Pay.Icbc
{
    public interface IIcbcpaySettings
    {
        /// <summary>
        /// 工行APPid
        /// </summary>
        string APP_ID { get; set; }
        /// <summary>
        /// 商户在微信开放平台注册的APPID
        /// </summary>
        string SHOP_APP_ID { get; set; }
        /// <summary>
        /// 商户id
        /// </summary>
        string MER_ID { get; set; }
        /// <summary>
        /// 收单产品协议编号
        /// </summary>
        string MER_PRTCL_NO { get; set; }

        /// <summary>
        ///     回调地址
        /// </summary>
        string NOTIFY_URL { get; set; }


        string MY_PRIVATE_KEY { get; set; }
        string APIGW_PUBLIC_KEY { get; set; }

    }
}

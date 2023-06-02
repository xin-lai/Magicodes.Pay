using System;
using System.Collections.Generic;
using System.Text;

namespace Magicodes.Pay.Icbc
{
    public class IcbcpaySettings: IIcbcpaySettings
    {
        /// <summary>
        /// 工行APPid
        /// </summary>
        public string APP_ID { get; set; }
        /// <summary>
        /// 商户在微信开放平台注册的APPID
        /// </summary>
        public string SHOP_APP_ID { get; set; }
        /// <summary>
        /// 商户id
        /// </summary>
        public string MER_ID { get; set; }
        /// <summary>
        /// 收单产品协议编号
        /// </summary>
        public string MER_PRTCL_NO { get; set; }

        /// <summary>
        ///     回调地址
        /// </summary>
        public string NOTIFY_URL { get; set; }


        public string MY_PRIVATE_KEY { get; set; }
        public string APIGW_PUBLIC_KEY { get; set; }

    }
}

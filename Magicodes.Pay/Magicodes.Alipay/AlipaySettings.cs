// ======================================================================
//   
//           Copyright (C) 2018-2020 湖南心莱信息科技有限公司    
//           All rights reserved
//   
//           filename : AlipaySettings.cs
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

namespace Magicodes.Alipay
{
    /// <summary>
    ///     支付宝支付设置
    /// </summary>
    public class AlipaySettings : IAlipaySettings
    {
        /// <summary>
        ///     应用ID,您的APPID
        /// </summary>
        public string AppId { get; set; }

        /// <summary>
        ///     合作商户uid
        /// </summary>
        public string Uid { get; set; }

        /// <summary>
        ///     支付宝网关
        /// </summary>
        public string Gatewayurl { get; set; } = "https://openapi.alipay.com/gateway.do";

        /// <summary>
        ///     商户私钥，您的原始格式RSA私钥
        /// </summary>
        public string PrivateKey { get; set; }

        /// <summary>
        ///     支付宝公钥,查看地址：https://openhome.alipay.com/platform/keyManage.htm 对应APPID下的支付宝公钥。
        /// </summary>
        public string AlipayPublicKey { get; set; }

        /// <summary>
        ///     支付宝签名公钥
        /// </summary>
        public string AlipaySignPublicKey { get; set; }

        /// <summary>
        ///     签名方式（默认值：RSA2）
        /// </summary>
        public string SignType { get; set; } = "RSA2";

        /// <summary>
        ///     编码格式（默认值：UTF-8）
        /// </summary>
        public string CharSet { get; set; } = "utf-8";

        /// <summary>
        ///     是否从文件读取公私钥 如果为true ，那么公私钥应该配置为密钥文件路径
        /// </summary>
        public bool IsKeyFromFile { get; set; } = false;

        /// <summary>
        ///     回调地址
        /// </summary>
        public string Notify { get; set; }
    }
}
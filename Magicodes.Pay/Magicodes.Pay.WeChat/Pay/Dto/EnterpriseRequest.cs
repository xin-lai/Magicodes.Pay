using System;
using System.Collections.Generic;
using System.Text;

namespace Magicodes.Pay.WeChat.Pay.Dto
{
    using System;
    using System.Xml.Serialization;

    /// <summary>
    /// Defines the <see cref="EnterpriseRequest" />
    /// </summary>
    [XmlRoot("xml")]
    [Serializable]
    public class EnterpriseRequest
    {
        /// <summary>
        /// Gets or sets the MchAppId
        /// 微信分配的公众账号ID（企业号corpid即为此appId）
        /// </summary>
        [XmlElement("mch_appid")]
        public string MchAppId { get; set; }

        /// <summary>
        /// Gets or sets the MchId
        /// 微信支付分配的商户号
        /// </summary>
        [XmlElement("mchid")]
        public string MchId { get; set; }

        /// <summary>
        /// Gets or sets the DeviceInfo
        /// 微信支付分配的终端设备号
        /// </summary>
        [XmlElement("device_info")]
        public string DeviceInfo { get; set; }

        /// <summary>
        /// Gets or sets the NonceStr
        /// 随机字符串，不长于32位
        /// </summary>
        [XmlElement("nonce_str")]
        public string NonceStr { get; set; }

        /// <summary>
        /// Gets or sets the Sign
        /// 签名
        /// </summary>
        [XmlElement("sign")]
        public string Sign { get; set; }

        /// <summary>
        /// Gets or sets the PartnerTradeNo
        /// 商户订单号，需保持唯一性
        /// </summary>
        [XmlElement("partner_trade_no")]
        public string PartnerTradeNo { get; set; }

        /// <summary>
        /// Gets or sets the OpenId
        /// 商户appid下，某用户的openid
        /// </summary>
        [XmlElement("openid")]
        public string OpenId { get; set; }

        /// <summary>
        /// Gets or sets the CheckName
        /// NO_CHECK：不校验真实姓名
        ///     FORCE_CHECK：强校验真实姓名（未实名认证的用户会校验失败，无法转账）
        ///     OPTION_CHECK：针对已实名认证的用户才校验真实姓名（未实名认证用户不校验，可以转账成功）
        /// </summary>
        [XmlElement("check_name")]
        public string CheckName { get; set; }

        /// <summary>
        /// Gets or sets the ReUserName
        /// 收款用户真实姓名。  如果check_name设置为FORCE_CHECK或OPTION_CHECK，则必填用户真实姓名
        /// </summary>
        [XmlElement("re_user_name")]
        public string ReUserName { get; set; }

        /// <summary>
        /// Gets or sets the Amount
        /// 企业付款金额，单位为分
        /// </summary>
        [XmlElement("amount")]
        public string Amount { get; set; }

        /// <summary>
        /// Gets or sets the Desc
        /// 企业付款操作说明信息。必填。
        /// </summary>
        [XmlElement("desc")]
        public string Desc { get; set; }

        /// <summary>
        /// Gets or sets the SpbillCreateIp
        /// 调用接口的机器Ip地址
        /// </summary>
        [XmlElement("spbill_create_ip")]
        public string SpbillCreateIp { get; set; }
    }
}

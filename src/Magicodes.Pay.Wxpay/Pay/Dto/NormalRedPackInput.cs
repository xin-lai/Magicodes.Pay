namespace Magicodes.Pay.Wxpay.Pay.Dto
{
    /// <summary>
    /// Defines the <see cref="NormalRedPackInput" />
    /// </summary>
    [XmlRoot("xml")]
    [Serializable]
    public class NormalRedPackInput
    {
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
        /// Gets or sets the MchBillno
        /// 户订单号（每个订单号必须唯一） 组成：mch_id+yyyymmdd+10位一天内不能重复的数字。接口根据商户订单号支持重入，如出现超时可再调用。
        /// </summary>
        [XmlElement("mch_billno")]
        public string MchBillno { get; set; }

        /// <summary>
        /// Gets or sets the MchId
        /// 微信支付分配的商户号
        /// </summary>
        [XmlElement("mch_id")]
        public string MchId { get; set; }

        /// <summary>
        /// Gets or sets the WxAppId
        /// 微信分配的公众账号ID（企业号corpid即为此appId）。接口传入的所有appid应该为公众号的appid（在mp.weixin.qq.com申请的），不能为APP的appid（在open.weixin.qq.com申请的）。
        /// </summary>
        [XmlElement("wxappid")]
        public string WxAppId { get; set; }

        /// <summary>
        /// Gets or sets the SendName
        /// 红包发送者名称
        /// </summary>
        [XmlElement("send_name")]
        public string SendName { get; set; }

        /// <summary>
        /// Gets or sets the ReOpenId
        /// 接受红包的用户 用户在wxappid下的openid
        /// </summary>
        [XmlElement("re_openid")]
        public string ReOpenId { get; set; }

        /// <summary>
        /// Gets or sets the TotalAmount
        /// 付款金额，单位分
        /// </summary>
        [XmlElement("total_amount")]
        public string TotalAmount { get; set; }

        /// <summary>
        /// Gets or sets the TotalNum
        /// 红包发放总人数，total_num=1
        /// </summary>
        [XmlElement("total_num")]
        public string TotalNum { get; set; }

        /// <summary>
        /// Gets or sets the Wishing
        /// 红包祝福语
        /// </summary>
        [XmlElement("wishing")]
        public string Wishing { get; set; }

        /// <summary>
        /// Gets or sets the ClientIp
        /// 调用接口的机器Ip地址
        /// </summary>
        [XmlElement("client_ip")]
        public string ClientIp { get; set; }

        /// <summary>
        /// Gets or sets the ActName
        /// 活动名称
        /// </summary>
        [XmlElement("act_name")]
        public string ActName { get; set; }

        /// <summary>
        /// Gets or sets the Remark
        /// 备注
        /// </summary>
        [XmlElement("remark")]
        public string Remark { get; set; }
    }
}

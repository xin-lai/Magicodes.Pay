namespace Magicodes.Pay.Wxpay.Pay.Dto
{
    /// <summary>
    /// Defines the <see cref="NormalRedPackOutput" />
    /// </summary>
    [XmlRoot("xml")]
    [Serializable]
    public class NormalRedPackOutput : PayOutputBase
    {
        /// <summary>
        /// Gets or sets the MchBillno
        /// 商户订单号（每个订单号必须唯一） 组成：mch_id+yyyymmdd+10位一天内不能重复的数字
        /// </summary>
        [XmlAttribute("mch_billno")]
        public string MchBillno { get; set; }

        /// <summary>
        /// Gets or sets the mch_id
        /// 商户号，微信支付分配的商户号
        /// </summary>
        [XmlAttribute("mch_id")]
        public string mchId { get; set; }

        /// <summary>
        /// Gets or sets the WxAppId
        /// 公众账号appid。商户appid，接口传入的所有appid应该为公众号的appid（在mp.weixin.qq.com申请的），不能为APP的appid（在open.weixin.qq.com申请的）。
        /// </summary>
        [XmlAttribute("wxappid")]
        public string WxAppId { get; set; }

        /// <summary>
        /// Gets or sets the ReOpenid
        /// 用户openid
        /// </summary>
        [XmlAttribute("re_openid")]
        public string ReOpenid { get; set; }

        /// <summary>
        /// Gets or sets the TotalAmount
        /// 付款金额，单位分
        /// </summary>
        [XmlAttribute("total_amount")]
        public string TotalAmount { get; set; }

        /// <summary>
        /// Gets or sets the SendListId
        /// 红包订单的微信单号
        /// </summary>
        [XmlAttribute("send_listid")]
        public string SendListId { get; set; }

        /// <summary>
        /// Gets or sets the Sign
        /// 签名
        /// </summary>
        [XmlAttribute("sign")]
        public string Sign { get; set; }
    }
}

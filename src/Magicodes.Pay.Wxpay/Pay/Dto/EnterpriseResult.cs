namespace Magicodes.Pay.Wxpay.Pay.Dto
{
    /// <summary>
    /// Defines the <see cref="EnterpriseResult" />
    /// </summary>
    [XmlRoot("xml")]
    [Serializable]
    public class EnterpriseResult : PayOutputBase
    {
        /// <summary>
        /// Gets or sets the result_code
        /// </summary>
        public string result_code { get; set; }

        /// <summary>
        /// Gets or sets the PartnerTradeNo
        /// 商户订单号，需保持唯一性
        /// </summary>
        [XmlAttribute("partner_trade_no")]
        public string PartnerTradeNo { get; set; }

        /// <summary>
        /// Gets or sets the PaymentNo
        /// 企业付款成功，返回的微信订单号
        /// </summary>
        [XmlAttribute("payment_no")]
        public string PaymentNo { get; set; }

        /// <summary>
        /// Gets or sets the PaymentTime
        /// 企业付款成功时间
        /// </summary>
        [XmlAttribute("payment_time")]
        public string PaymentTime { get; set; }
    }
}

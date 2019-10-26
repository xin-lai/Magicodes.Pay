namespace Magicodes.Pay.Wxpay.Pay.Dto
{
    /// <summary>
    /// Defines the <see cref="RefundRequest" />
    /// </summary>
    [Serializable]
    [XmlRoot("xml")]
    public class RefundRequest
    {
        /// <summary>
        /// Gets or sets the AppId
        /// 微信分配的公众账号ID
        /// </summary>
        [XmlElement("appid")]
        public string AppId { get; set; }

        /// <summary>
        /// Gets or sets the Mch_Id
        /// 微信支付分配的商户号
        /// </summary>
        [XmlElement("mch_id")]
        public string MchId { get; set; }

        /// <summary>
        /// Gets or sets the Device_Info
        /// 微信支付分配的终端设备号
        /// </summary>
        [XmlElement("device_info")]
        public string DeviceInfo { get; set; }

        /// <summary>
        /// Gets or sets the NonceStr
        /// 随机字符串，不长于32 位
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
        /// Gets or sets the Transaction_id
        /// 微信生成的订单号，在支付通知中有返回
        /// </summary>
        [XmlElement("transaction_id")]
        public string TransactionId { get; set; }

        /// <summary>
        /// Gets or sets the Out_trade_no
        /// 商户传给微信的订单号
        /// </summary>
        [XmlElement("out_trade_no")]
        public string OutTradeNo { get; set; }

        /// <summary>
        /// Gets or sets the Out_refund_no
        /// 商户系统内部的退款单号，商户系统内部唯一，同一退款单号多次请求只退一笔
        /// </summary>
        [XmlElement("out_refund_no")]
        public string OutRefundNo { get; set; }

        /// <summary>
        /// Gets or sets the Total_fee
        /// 订单总金额，单位为分，只能为整数
        /// </summary>
        [XmlElement("total_fee")]
        public int TotalFee { get; set; }

        /// <summary>
        /// Gets or sets the Refund_fee
        /// 退款总金额，订单总金额，单位为分，只能为整数
        /// </summary>
        [XmlElement("refund_fee")]
        public int RefundFee { get; set; }

        /// <summary>
        /// Gets or sets the Refund_fee_type
        /// 货币类型，符合ISO 4217标准的三位字母代码，默认人民币：CNY，其他值列表详见货币类型
        /// </summary>
        [XmlElement("refund_fee_type")]
        public string RefundFeeType { get; set; }

        /// <summary>
        /// Gets or sets the Op_user_id
        /// 操作员帐号, 默认为商户号
        /// </summary>
        [XmlElement("op_user_id")]
        public string OpUserId { get; set; }

        /// <summary>
        /// Gets or sets the Refund_account
        /// 仅针对老资金流商户使用REFUND_SOURCE_UNSETTLED_FUNDS---未结算资金退款（默认使用未结算资金退款）REFUND_SOURCE_RECHARGE_FUNDS---可用余额退款
        /// </summary>
        [XmlElement("refund_account")]
        public string RefundAccount { get; set; }
    }
}

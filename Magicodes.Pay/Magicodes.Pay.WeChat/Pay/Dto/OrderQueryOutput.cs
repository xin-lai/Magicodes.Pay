using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using Magicodes.Pay.WeChat.Pay.Dto;

namespace Magicodes.Pay.WeChat.Pay.Models
{
    [XmlRoot("xml")]
    [Serializable]
    public class OrderQueryOutput : PayOutputBase
    {
        /// <summary>
        ///     交易类型:JSAPI、NATIVE、APP
        /// </summary>
        [XmlElement("trade_type")]
        public string TradeType { get; set; }

        /// <summary>
        ///     微信分配的公众账号ID
        /// </summary>
        [XmlElement("appid")]
        public string AppId { get; set; }

        /// <summary>
        ///     微信支付分配的商户号
        /// </summary>
        [XmlElement("mch_id")]
        public string Mch_Id { get; set; }

        /// <summary>
        ///     微信支付分配的终端设备号
        /// </summary>
        [XmlElement("device_info")]
        public string Device_Info { get; set; }

        /// <summary>
        ///     随机字符串，不长于32 位
        /// </summary>
        [XmlElement("nonce_str")]
        public string NonceStr { get; set; }

        /// <summary>
        ///     签名
        /// </summary>
        [XmlElement("sign")]
        public string Sign { get; set; }

        /// <summary>
        /// 用户在商户appid下的唯一标识
        /// </summary>
        [XmlElement("openid")]
        public string OpenId { get; set; }

        /// <summary>
        /// 用户是否关注公众账号，Y-关注，N-未关注，仅在公众账号类型支付有效
        /// </summary>
        [XmlElement("is_subscribe")]
        public string IsSubscribe { get; set; }

        /// <summary>
        /// SUCCESS—支付成功
        /// REFUND—转入退款
        /// NOTPAY—未支付
        /// CLOSED—已关闭
        /// REVOKED—已撤销（刷卡支付）
        /// USERPAYING--用户支付中
        /// PAYERROR--支付失败(其他原因，如银行返回失败)
        /// 支付状态机请见下单API页面
        /// </summary>
        [XmlElement("trade_state")]
        public string TradeState { get; set; }

        /// <summary>
        /// 银行类型，采用字符串类型的银行标识
        /// </summary>
        [XmlElement("bank_type")]
        public string BankType { get; set; }

        /// <summary>
        /// 订单总金额，单位为分
        /// </summary>
        [XmlElement("total_fee")]
        public int TotalFee { get; set; }

        /// <summary>
        /// 当订单使用了免充值型优惠券后返回该参数，应结订单金额=订单金额-免充值优惠券金额。
        /// </summary>
        [XmlElement("settlement_total_fee")]
        public int SettlementTotalFee { get; set; }

        /// <summary>
        /// 货币类型，符合ISO 4217标准的三位字母代码，默认人民币：CNY，其他值列表详见货币类型
        /// </summary>
        [XmlElement("fee_type")]
        public string FeeType { get; set; }

        /// <summary>
        /// 现金支付金额订单现金支付金额，详见支付金额
        /// </summary>
        [XmlElement("cash_fee")]
        public string CashFee { get; set; }

        /// <summary>
        /// 货币类型，符合ISO 4217标准的三位字母代码，默认人民币：CNY，其他值列表详见货币类型
        /// </summary>
        [XmlElement("cash_fee_type")]
        public string CashFeeType { get; set; }

        /// <summary>
        /// <![CDATA[代金券金额<=订单金额，订单金额-代金券金额=现金支付金额，详见支付金额]]]>
        /// </summary>
        [XmlElement("coupon_fee")]
        public string CouponFee { get; set; }

        /// <summary>
        ///     代金券使用数量
        /// </summary>
        [XmlElement("coupon_count")]
        public string CouponCount { get; set; }

        /// <summary>
        ///     CASH--充值代金券         NO_CASH---非充值代金券        订单使用代金券时有返回（取值：CASH、NO_CASH）。$n为下标,从0开始编号，举例：coupon_type_$0
        /// </summary>
        [XmlElement("coupon_type_$n")]
        public string CouponTypeN { get; set; }

        /// <summary>
        ///     代金券ID,$n为下标，从0开始编号
        /// </summary>
        [XmlElement("coupon_id_$n")]
        public string CouponIdN { get; set; }

        /// <summary>
        ///     单个代金券支付金额,$n为下标，从0开始编号
        /// </summary>
        [XmlElement("coupon_fee_$n")]
        public string CouponFeeN { get; set; }

        /// <summary>
        ///     微信支付订单号
        /// </summary>
        [XmlElement("transaction_id")]
        public string TransactionId { get; set; }

        /// <summary>
        ///     商户系统的订单号，与请求一致
        /// </summary>
        [XmlElement("out_trade_no")]
        public string OutTradeNo { get; set; }

        /// <summary>
        ///     商家数据包，原样返回
        /// </summary>
        [XmlElement("attach")]
        public string Attach { get; set; }

        /// <summary>
        ///     支付完成时间，格式为yyyyMMddHHmmss，如2009年12月25日9点10分10秒表示为20091225091010。其他详见时间规则
        /// </summary>
        [XmlElement("time_end")]
        public string TimeEnd { get; set; }

        /// <summary>
        /// 对当前查询订单状态的描述和下一步操作的指引,如:支付失败，请重新下单支付
        /// </summary>
        [XmlElement("trade_state_desc")]
        public string TradeStateDesc { get; set; }
    }
}

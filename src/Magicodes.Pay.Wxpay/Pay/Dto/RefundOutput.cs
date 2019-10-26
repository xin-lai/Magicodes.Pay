// ======================================================================
//   
//           Copyright (C) 2019-2020 湖南心莱信息科技有限公司    
//           All rights reserved
//   
//           filename : RefundOutput.cs
//           description :
//   
//           created by 雪雁 at  2019-01-17 13:49
//           Mail: wenqiang.li@xin-lai.com
//           QQ群：85318032（技术交流）
//           Blog：http://www.cnblogs.com/codelove/
//           GitHub：https://github.com/xin-lai
//           Home：http://xin-lai.com
//   
// ======================================================================

using System;
using System.Xml.Serialization;

namespace Magicodes.Pay.Wxpay.Pay.Dto
{
    /// <summary>
    ///     申请退款return_code为SUCCESS的时候有返回
    /// </summary>
    [XmlRoot("xml")]
    [Serializable]
    public class RefundOutput : PayOutputBase
    {
        /// <summary>
        ///     Gets or sets the Transaction_id
        ///     微信订单号	是	String(28)	4007752501201407033233368018	微信订单号
        /// </summary>
        [XmlElement("transaction_id")]
        public string TransactionId { get; set; }

        /// <summary>
        ///     Gets or sets the Out_trade_no
        ///     商户订单号	是	String(32)	33368018	商户系统内部的订单号
        /// </summary>
        [XmlElement("out_trade_no")]
        public string OutTradeNo { get; set; }

        /// <summary>
        ///     Gets or sets the Out_refund_no
        ///     商户退款单号	是	String(32)	121775250	商户退款单号
        /// </summary>
        [XmlElement("out_refund_no")]
        public string OutRefundNo { get; set; }

        /// <summary>
        ///     Gets or sets the Refund_id
        ///     微信退款单号	是	String(28)	2007752501201407033233368018	微信退款单号
        /// </summary>
        [XmlElement("refund_id")]
        public string RefundId { get; set; }

        /// <summary>
        ///     Gets or sets the Refund_channel
        ///     退款渠道	否	String(16)	ORIGINAL	ORIGINAL—原路退款 BALANCE—退回到余额
        /// </summary>
        [XmlElement("refund_channel")]
        public string RefundChannel { get; set; }

        /// <summary>
        ///     Gets or sets the Refund_fee
        ///     申请退款金额	是	Int	100	退款总金额,单位为分,可以做部分退款
        /// </summary>
        [XmlElement("refund_fee")]
        public int RefundFee { get; set; }

        /// <summary>
        ///     Gets or sets the Settlement_refund_fee
        ///     退款金额	否	Int	100	去掉非充值代金券退款金额后的退款金额，退款金额=申请退款金额-非充值代金券退款金额，退款金额<=申请退款金额
        /// </summary>
        [XmlElement("settlement_refund_fee")]
        public int SettlementRefundFee { get; set; }

        /// <summary>
        ///     Gets or sets the Total_fee
        ///     订单金额	是	Int	100	订单总金额，单位为分，只能为整数，详见支付金额
        /// </summary>
        [XmlElement("total_fee")]
        public int TotalFee { get; set; }

        /// <summary>
        ///     Gets or sets the Settlement_total_fee
        ///     应结订单金额	否	Int	100	去掉非充值代金券金额后的订单总金额，应结订单金额=订单金额-非充值代金券金额，应结订单金额<=订单金额。
        /// </summary>
        [XmlElement("settlement_total_fee")]
        public int SettlementTotalFee { get; set; }

        /// <summary>
        ///     Gets or sets the Fee_type
        ///     订单金额货币种类	否	String(8)	CNY	订单金额货币类型，符合ISO 4217标准的三位字母代码，默认人民币：CNY，其他值列表详见货币类型
        /// </summary>
        [XmlElement("fee_type")]
        public string FeeType { get; set; }

        /// <summary>
        ///     Gets or sets the Cash_fee
        ///     现金支付金额	是	Int	100	现金支付金额，单位为分，只能为整数，详见支付金额
        /// </summary>
        [XmlElement("cash_fee")]
        public int CashFee { get; set; }

        /// <summary>
        ///     Gets or sets the Cash_refund_fee
        ///     现金退款金额	否	Int	100	现金退款金额，单位为分，只能为整数，详见支付金额
        /// </summary>
        [XmlElement("cash_refund_fee")]
        public int CashRefundFee { get; set; }

        /// <summary>
        ///     Gets or sets the Coupon_type_0
        ///     代金券类型	否	String(8)	CASH	CASH--充值代金券 NO_CASH---非充值代金券 订单使用代金券时有返回（取值：CASH、NO_CASH）。$n为下标,从0开始编号，举例：coupon_type_0
        /// </summary>
        [XmlElement("coupon_type_0")]
        public string CouponType0 { get; set; }

        /// <summary>
        ///     Gets or sets the Coupon_refund_fee_0
        ///     代金券退款金额	否	Int	100	代金券退款金额小于等于退款金额，退款金额-代金券或立减优惠退款金额为现金，说明详见代金券或立减优惠
        /// </summary>
        [XmlElement("coupon_refund_fee_0")]
        public int CouponRefundFee0 { get; set; }

        /// <summary>
        ///     Gets or sets the Coupon_refund_count_0
        ///     退款代金券使用数量	否	Int	1	退款代金券使用数量 ,$n为下标,从0开始编号
        /// </summary>
        [XmlElement("coupon_refund_count_0")]
        public int CouponRefundCount0 { get; set; }

        /// <summary>
        ///     Gets or sets the Coupon_refund_batch_id_0_0
        ///     退款代金券批次ID	否	String(20)	100	退款代金券批次ID ,$n为下标，$m为下标，从0开始编号
        /// </summary>
        [XmlElement("coupon_refund_batch_id_0_0")]
        public string CouponRefundBatchId00 { get; set; }

        /// <summary>
        ///     Gets or sets the Coupon_refund_id_0_0
        ///     退款代金券ID	否	String(20)	10000 	退款代金券ID, $n为下标，$m为下标，从0开始编号
        /// </summary>
        [XmlElement("coupon_refund_id_0_0")]
        public string CouponRefundId00 { get; set; }

        /// <summary>
        ///     Gets or sets the Coupon_refund_fee_0_0
        ///     单个退款代金券支付金额	否	Int	100	单个退款代金券支付金额, $n为下标，$m为下标，从0开始编号
        /// </summary>
        [XmlElement("coupon_refund_fee_0_0")]
        public int CouponRefundFee00 { get; set; }

        /// <summary>
        /// Gets or sets the Sign
        /// 签名
        /// </summary>
        [XmlAttribute("sign")]
        public string Sign { get; set; }
    }
}
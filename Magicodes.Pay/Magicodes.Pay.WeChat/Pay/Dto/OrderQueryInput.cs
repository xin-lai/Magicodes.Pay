using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace Magicodes.Pay.WeChat.Pay.Dto
{
    /// <summary>
    /// 订单查询
    /// </summary>
    public class OrderQueryInput
    {
        /// <summary>
        ///     微信支付订单号（二选一）
        /// </summary>
        [XmlElement("transaction_id")]
        public string TransactionId { get; set; }

        /// <summary>
        ///     商户系统的订单号，与请求一致（二选一）
        /// </summary>
        [XmlElement("out_trade_no")]
        public string OutTradeNo { get; set; }
    }
}

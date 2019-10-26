// ======================================================================
//   
//           Copyright (C) 2018-2020 湖南心莱信息科技有限公司    
//           All rights reserved
//   
//           filename : AppUnifiedorderRequest.cs
//           description :
//   
//           created by 雪雁 at  2018-07-30 17:45
//           Mail: wenqiang.li@xin-lai.com
//           QQ群：85318032（技术交流）
//           Blog：http://www.cnblogs.com/codelove/
//           GitHub：https://github.com/xin-lai
//           Home：http://xin-lai.com
//   
// ======================================================================

namespace Magicodes.Pay.Wxpay.Pay.Models
{
    public class AppUnifiedorderRequest
    {
        /// <summary>
        ///     微信开放平台审核通过的应用APPID
        /// </summary>
        [XmlElement("appid")]
        public string AppId { get; set; }

        /// <summary>
        ///     微信支付分配的商户号
        /// </summary>
        [XmlElement("mch_id")]
        public string MchId { get; set; }

        /// <summary>
        ///     随机字符串，不长于32位
        /// </summary>
        [XmlElement("nonce_str")]
        public string NonceStr { get; set; }

        /// <summary>
        ///     签名
        /// </summary>
        [XmlElement("sign")]
        public string Sign { get; set; }

        /// <summary>
        ///     商品描述交易字段格式根据不同的应用场景按照以下格式： APP——需传入应用市场上的APP名字-实际商品名称，天天爱消除-游戏充值。
        /// </summary>
        [XmlElement("body")]
        public string Body { get; set; }

        /// <summary>
        ///     商品名称明细列表
        /// </summary>
        [XmlElement("detail")]
        public string Detail { get; set; }

        /// <summary>
        ///     附加数据，在查询API和支付通知中原样返回，该字段主要用于商户携带订单的自定义数据
        /// </summary>
        [XmlElement("attach")]
        public string Attach { get; set; }

        /// <summary>
        ///     商户系统内部的订单号,32个字符内、可包含字母, 其他说明见商户订单号
        /// </summary>
        [XmlElement("out_trade_no")]
        public string OutTradeNo { get; set; }

        /// <summary>
        ///     符合ISO 4217标准的三位字母代码，默认人民币：CNY
        /// </summary>
        [XmlElement("fee_type")]
        public string FeeType { get; set; }

        /// <summary>
        ///     订单总金额，单位为分
        /// </summary>
        [XmlElement("total_fee")]
        public string TotalFee { get; set; }

        /// <summary>
        ///     用户端实际ip
        /// </summary>
        [XmlElement("spbill_create_ip")]
        public string SpbillCreateIp { get; set; }

        /// <summary>
        ///     订单生成时间，格式为yyyyMMddHHmmss，如2009年12月25日9点10分10秒表示为20091225091010。
        /// </summary>
        [XmlElement("time_start")]
        public string TimeStart { get; set; }

        /// <summary>
        ///     订单失效时间，格式为yyyyMMddHHmmss，如2009年12月27日9点10分10秒表示为20091227091010
        /// </summary>
        [XmlElement("time_expire")]
        public string TimeExpire { get; set; }

        /// <summary>
        ///     商品标记，代金券或立减优惠功能的参数，说明详见代金券或立减优惠
        /// </summary>
        [XmlElement("goods_tag")]
        public string GoodsTag { get; set; }

        /// <summary>
        ///     【不用填写】接收微信支付异步通知回调地址，通知url必须为直接可访问的url，不能携带参数
        /// </summary>
        [XmlElement("notify_url")]
        public string NotifyUrl { get; set; }

        /// <summary>
        ///     支付类型（JSAPI，NATIVE，APP）公众号内支付填JSAPI
        /// </summary>
        [XmlElement("trade_type")]
        public string TradeType { get; set; } = "APP";

        /// <summary>
        ///     no_credit--指定不能使用信用卡支付
        /// </summary>
        [XmlElement("limit_pay")]
        public string LimitPay { get; set; }
    }
}
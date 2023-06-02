using Magicodes.Pay.Icbcpay; 
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Magicodes.Pay.Icbc.Dto
{

    public class CardbusinessAggregatepayB2cOnlineConsumepurchaseResponseV1 : IcbcResponse
    {
        ///// <summary>
        ///// 返回码，交易成功返回0，其余为失败情况	
        ///// </summary>
        //public string return_code { get; set; }
        ///// <summary>
        ///// 返回信息描述	
        ///// </summary>
        //public string return_msg { get; set; }
        /// <summary>
        /// 用于确定一笔签名的唯一性，不能重复	
        /// </summary>
        public string msg_id
        { get; set; }
        /// <summary>
        /// 订单总金额，一笔订单一个，以分为单位。不可以为零，必需符合金额标准	
        /// </summary>
        public string total_amt
        { get; set; }
        /// <summary>
        /// 商户系统订单号，原样返回	
        /// </summary>
        public string out_trade_no
        { get; set; }
        /// <summary>
        /// 工行订单号	
        /// </summary>
        public string order_id
        { get; set; }
        /// <summary>
        /// 支付完成时间，格式为：yyyyMMdd	
        /// </summary>
        public string pay_time
        { get; set; }
        /// <summary>
        /// 商户编号	
        /// </summary>
        public string mer_id
        { get; set; }
        /// <summary>
        /// 支付方式，1-刷卡支付；2-工行快捷支付；3-网银支付；4-新型无卡；5-简单无卡；6-银联快捷支付；7-3D支付；8-ApplePay；9-微信；10-支付宝；11-非3D支付；12-银联系扫码付；13-云闪付	
        /// </summary>
        public string pay_mode
        { get; set; }
        /// <summary>
        /// 收单接入方式，5-APP，7-微信公众号，8-支付宝生活号，9-微信小程序	
        /// </summary>
        public string access_type
        { get; set; }
        /// <summary>
        /// 卡种，90-VISA外卡、91-MASTER外卡、92-运通外卡、93-大来外卡、94-JCB外卡	
        /// </summary>
        public string card_kind
        { get; set; }
        /// <summary>
        /// 支付方式为微信时返回，交易类型，JSAPI ：公众号支付、小程序支付；APP：APP支付；	
        /// </summary>
        public string trade_type
        { get; set; }
        /// <summary>
        /// 支付方式为微信时返回，微信数据包，用于之后唤起微信支付。详细内容请参考微信支付开放平台接口	
        /// </summary>
        public string wx_data_package
        { get; set; }
        /// <summary>
        /// 支付方式为支付宝时返回，支付宝数据包，用于之后唤起支付宝支付。详细内容请参考支付宝开放平台接口	
        /// </summary>
        public string zfb_data_package
        { get; set; }
        /// <summary>
        /// 支付方式为云闪付时返回，云闪付受理订单号，用于之后进行银联云闪付支付。详细内容请参考银联开放平台的手机支付控件接口	
        /// </summary>
        public string union_data_package
        { get; set; }
        /// <summary>
        /// 第三方报错时返回的报错码	
        /// </summary>
        public string third_party_return_code
        { get; set; }
        /// <summary>
        /// 第三方报错时返回的报错信息

        /// </summary>
        public string third_party_return_msg
        { get; set; }
    }
}

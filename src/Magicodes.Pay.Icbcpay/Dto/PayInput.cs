using System;
using System.Collections.Generic;
using System.Text;

namespace Magicodes.Pay.Icbc.Dto
{
    public class PayInput
    {
        /// <summary>
        ///     商户交易单号
        /// </summary>
        public string OrderNumber { get; set; }

        /// <summary>
        ///     交易金额(单位为分)
        /// </summary>
        public int Amount { get; set; }

        /// <summary>
        ///     订单标题
        /// </summary>
        public string Body { get; set; }

        /// <summary>
        ///     备注信息
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        ///  设备号
        /// </summary>
        public string DeciveInfo { get; set; } = "001";

        /// <summary>
        ///     支付平台用户标识
        ///     JS支付时使用
        ///     微信支付-用户的微信openid
        ///     支付宝支付-用户user_id
        ///     微信小程序-用户小程序的openid
        /// </summary>
        public string OpenId { get; set; }

        /// <summary>
        /// 支付方式，9-微信；10-支付宝；13-云闪付	
        /// </summary>
        public string PayModel { get; set; } = "9";

        /// <summary>
        /// 收单接入方式，5-APP，7-微信公众号，8-支付宝生活号，9-微信小程序	
        /// </summary>
        public string AccessType { get; set; } = "9";
        /// <summary>
        /// 用户端IP
        /// </summary>
        public string SpbillCreateIp { get; set; }

        /// <summary>
        /// 第三方用户标识，商户在支付宝生活号接入时必送，即access_type为8时，上送用户的唯一标识；商户通过微信公众号内或微信小程序接入时不送	
        /// </summary> 
        public string UnionId;
        /// <summary>
        ///     订单有效时间，以秒为单位 
        /// </summary>
        public string ValidTime { get; set; }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString() => Newtonsoft.Json.JsonConvert.SerializeObject(this);

    }
}

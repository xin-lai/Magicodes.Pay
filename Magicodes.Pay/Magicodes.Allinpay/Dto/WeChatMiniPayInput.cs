using System;
using Newtonsoft.Json;

namespace Magicodes.Allinpay.Dto
{
    public class WeChatMiniPayInput
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
        ///     订单有效时间，以分为单位，不填默认为5分钟,最大2880分钟
        /// </summary>
        public string ValidTime { get; set; }

        /// <summary>
        ///     支付平台用户标识
        ///     JS支付时使用
        ///     微信支付-用户的微信openid
        ///     支付宝支付-用户user_id
        ///     微信小程序-用户小程序的openid
        /// </summary>
        public string OpenId { get; set; }

        /// <summary>
        ///     接收微信支付异步通知回调地址，通知url必须为直接可访问的url，不能携带参数
        /// </summary>
        public string NotifyUrl { get; set; }

        /// <summary>
        ///     微信小程序/微信公众号/APP的appid
        /// </summary>
        public string WeChatAppId { get; set; }
    }
}
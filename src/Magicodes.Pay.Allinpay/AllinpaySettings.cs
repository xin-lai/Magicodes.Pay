namespace Magicodes.Pay.Allinpay
{
    /// <summary>
    ///     通联支付设置
    /// </summary>
    public class AllinpaySettings : IAllinpaySettings
    {
        /// <summary>
        ///     实际交易的商户号
        /// </summary>
        public string CusId { get; set; }

        /// <summary>
        ///     平台分配的APPID
        /// </summary>
        public string AppId { get; set; }

        /// <summary>
        ///     平台分配的AppKey
        /// </summary>
        public string AppKey { get; set; }

        /// <summary>
        ///     接口网关
        /// </summary>
        public string ApiGateWay { get; set; }

        /// <summary>
        ///     版本
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        ///     回调地址
        /// </summary>
        public string NotifyUrl { get; set; }

        /// <summary>
        ///     微信AppId
        /// </summary>
        public string WeChatAppId { get; set; }

        /// <summary>
        ///     JsApiAppId
        /// </summary>
        public string JsApiAppId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString() => Newtonsoft.Json.JsonConvert.SerializeObject(this);
    }
}
namespace Magicodes.Pay.Allinpay
{
    /// <summary>
    ///     通联支付设置
    /// </summary>
    public interface IAllinpaySettings
    {
        /// <summary>
        ///     实际交易的商户号
        /// </summary>
        string CusId { get; set; }

        /// <summary>
        ///     平台分配的APPID
        /// </summary>
        string AppId { get; set; }

        /// <summary>
        ///     平台分配的AppKey
        /// </summary>
        string AppKey { get; set; }

        /// <summary>
        ///     接口网关
        /// </summary>
        string ApiGateWay { get; set; }

        /// <summary>
        ///     版本
        /// </summary>
        string Version { get; set; }

        /// <summary>
        ///     回调地址
        /// </summary>
        string NotifyUrl { get; set; }

        /// <summary>
        ///     微信AppId
        /// </summary>
        string WeChatAppId { get; set; }

        /// <summary>
        ///     JsApiAppId
        /// </summary>
        string JsApiAppId { get; set; }
    }
}
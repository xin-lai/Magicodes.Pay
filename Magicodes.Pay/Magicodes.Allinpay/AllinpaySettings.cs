namespace Magicodes.Allinpay
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
        public string ApiGateWay { get; set; } = "https://vsp.allinpay.com/apiweb/unitorder";

        /// <summary>
        ///     版本
        /// </summary>
        public string Version { get; set; } = "11";
    }
}
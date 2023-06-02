namespace Magicodes.Pay.Volo.Abp.TransactionLogs
{
    /// <summary>
    /// 支付渠道
    /// </summary>
    public enum PayChannels
    {
        /// <summary>
        /// 微信支付
        /// </summary>
        WeChatPay = 0,

        /// <summary>
        /// 支付宝APP支付
        /// </summary>
        AliAppPay = 1,

        /// <summary>
        /// 余额支付
        /// </summary>
        BalancePay = 2,

        /// <summary>
        /// 国际支付宝
        /// </summary>
        GlobalAlipay = 3,

        /// <summary>
        /// 微信小程序支付
        /// </summary>
        WeChatMiniProgram = 4,

        /// <summary>
        /// 通联微信小程序
        /// </summary>
        AllinWeChatMiniPay = 5,

        /// <summary>
        /// 微信APP支付
        /// </summary>
        WeChatAppPay = 6,

        /// <summary>
        /// 通联JSAPI
        /// </summary>
        AllinJsApiPay = 7,
        /// <summary>
        /// 农业银行
        /// </summary>
        AbcPay = 8,
        /// <summary>
        /// 工商银行
        /// </summary>
        IcbcPay=9
    }
}
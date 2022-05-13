namespace Magicodes.Pay.Volo.Abp.TransactionLogs
{
    /// <summary>
    /// 交易状态
    /// </summary>
    public enum TransactionStates
    {
        /// <summary>
        /// 未支付
        /// </summary>
        NotPay = 0,

        /// <summary>
        /// 支付成功
        /// </summary>
        Success = 1,

        /// <summary>
        /// 支付失败（支付接口问题等）
        /// </summary>
        PayError = 2,

        /// <summary>
        /// 待退款
        /// </summary>
        PendingRefund = 3,

        /// <summary>
        /// 已退款
        /// </summary>
        Refunded = 4
    }
}
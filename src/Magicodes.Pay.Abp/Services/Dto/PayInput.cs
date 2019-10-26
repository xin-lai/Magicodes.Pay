using Magicodes.Pay.Abp.TransactionLogs;

namespace Magicodes.Pay.Abp.Services.Dto
{
    public class PayInput : AppPayInput
    {
        /// <summary>
        /// 支付渠道
        /// </summary>
        public PayChannels PayChannel { get; set; }

        /// <summary>
        /// 业务Key
        /// </summary>
        public string Key { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace Magicodes.Pay.Volo.Abp.Dto
{
    /// <summary>
    /// 
    /// </summary>
    public class ExecPayNotifyOutputDto
    {
        /// <summary>
        /// 外部交易单号（商户单号）
        /// </summary>
        public string OutTradeNo { get; set; }

        /// <summary>
        /// 交易单号
        /// </summary>
        public string TradeNo { get; set; }

        /// <summary>
        /// 交易金额（单位：元）
        /// </summary>
        public decimal TotalFee { get; set; }

        /// <summary>
        /// 业务参数
        /// </summary>
        public string BusinessParams { get; set; }

        /// <summary>
        /// 成功通知
        /// </summary>
        public object SuccessResult { get; set; }
    }
}

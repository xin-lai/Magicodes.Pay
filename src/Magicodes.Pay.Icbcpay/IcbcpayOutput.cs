using System;
using System.Collections.Generic;
using System.Text;

namespace Magicodes.Pay.Icbcpay
{
    public class IcbcpayOutput
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
        public string SuccessResult { get; set; }

    }

    public class IcbcpayResult {

        public BizContentResult response_biz_content { get; set; }
        public string sign_type { get; set; }

        public string sign { get; set; }
    }

    public class BizContentResult { 
        public string return_code { get; set; }

        public string return_msg { get; set; }

        public string msg_id { get; set; }
    }

}

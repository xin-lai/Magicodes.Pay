// ======================================================================
//   
//           Copyright (C) 2018-2020 湖南心莱信息科技有限公司    
//           All rights reserved
//   
//           filename : TransactionInfo.cs
//           description :
//   
//           created by 雪雁 at  2018-08-06 14:21
//           Mail: wenqiang.li@xin-lai.com
//           QQ群：85318032（技术交流）
//           Blog：http://www.cnblogs.com/codelove/
//           GitHub：https://github.com/xin-lai
//           Home：http://xin-lai.com
//   
// ======================================================================

using System;
using System.ComponentModel.DataAnnotations;

namespace Magicodes.Pay.Abp.TransactionLogs
{
    public class TransactionInfo
    {
        /// <summary>
        ///     金额
        /// </summary>
        [Display(Name = "金额")]
        public decimal Amount { get; set; }

        /// <summary>
        ///     支付渠道
        /// </summary>
        [Display(Name = "支付渠道")]
        public PayChannels PayChannel { get; set; }

        /// <summary>
        ///     交易状态
        /// </summary>
        [Display(Name = "交易状态")]
        public TransactionStates TransactionState { get; set; }

        /// <summary>
        ///     自定义数据
        /// </summary>
        [MaxLength(500)]
        [Display(Name = "自定义数据")]
        public string CustomData { get; set; }

        /// <summary>
        ///     交易单号
        /// </summary>
        [MaxLength(50)]
        [Display(Name = "交易单号")]
        public string OutTradeNo { get; set; }

        /// <summary>
        ///     支付订单号（比如微信支付订单号）
        /// </summary>
        [MaxLength(50)]
        [Display(Name = "支付订单号")]
        public string TransactionId { get; set; }

        /// <summary>
        ///     支付完成时间
        /// </summary>
        [Display(Name = "支付完成时间")]
        public DateTime? PayTime { get; set; }

        /// <summary>
        ///     异常信息
        /// </summary>
        public Exception Exception { get; set; }

        /// <summary>
        /// 主题（商品的标题/交易标题/订单标题/订单关键字等）
        /// </summary>
        [MaxLength(256)]
        public string Subject { get; set; }
    }
}
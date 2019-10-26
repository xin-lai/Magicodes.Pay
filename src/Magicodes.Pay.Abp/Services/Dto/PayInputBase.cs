using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Magicodes.Pay.Abp.TransactionLogs;

namespace Magicodes.Pay.Abp.Services.Dto
{
    /// <summary>
    /// 基本支付输入参数
    /// </summary>
    public class PayInputBase
    {
        /// <summary>
        /// 支付渠道
        /// </summary>
        public PayChannels PayChannel { get; set; }

        /// <summary>
        /// 业务Key
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        ///     对一笔交易的具体描述信息。如果是多种商品，请将商品描述字符串累加传给body。
        /// </summary>
        [MaxLength(128)]
        public string Body { get; set; }

        /// <summary>
        ///     商品的标题/交易标题/订单标题/订单关键字等。
        /// </summary>
        [Required]
        [MaxLength(256)]
        public string Subject { get; set; }

        /// <summary>订单总金额，单位为元，精确到小数点后两位，取值范围[0.01,100000000]</summary>
        [Required]
        [Range(0.01, 100000000)]
        public decimal TotalAmount { get; set; }

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
        ///     OpenId
        /// </summary>
        [MaxLength(50)]
        [Display(Name = "OpenId")]
        public string OpenId { get; set; }
    }
}

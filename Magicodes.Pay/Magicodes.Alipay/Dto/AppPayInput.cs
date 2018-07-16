using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Magicodes.Alipay.Dto
{
    public class AppPayInput
    {
        /// <summary>
        /// 对一笔交易的具体描述信息。如果是多种商品，请将商品描述字符串累加传给body。
        /// </summary>
        [MaxLength(128)]
        public string Body { get; set; }

        /// <summary>
        /// 商品的标题/交易标题/订单标题/订单关键字等。
        /// </summary>
        [Required]
        [MaxLength(256)]
        public string Subject { get; set; }

        /// <summary>
        /// 商户网站唯一订单号（为空则自动生成）
        /// </summary>
        [MaxLength(64)]
        public string TradeNo { get; set; }

        /// <summary>
        /// 该笔订单允许的最晚付款时间，逾期将关闭交易。取值范围：1m～15d。m-分钟，h-小时，d-天，1c-当天（1c-当天的情况下，无论交易何时创建，都在0点关闭）。 该参数数值不接受小数点， 如 1.5h，可转换为 90m。
        /// 注：若为空，则默认为15d。
        /// </summary>
        [MaxLength(6)]
        public string TimeoutExpress { get; set; }

        /// <summary>
        /// 订单总金额，单位为元，精确到小数点后两位，取值范围[0.01,100000000]
        /// </summary>
        [Required]
        [MaxLength(9)]
        [Range(0.01, 100000000)]
        public decimal TotalAmount { get; set; }

        /// <summary>
        /// 商品主类型：0—虚拟类商品，1—实物类商品
        /// 注：虚拟类商品不支持使用花呗渠道
        /// </summary>
        [MaxLength(2)]
        public string GoodsType { get; set; }

        /// <summary>
        /// 公用回传参数，如果请求时传递了该参数，则返回给商户时会回传该参数。支付宝会在异步通知时将该参数原样返回。本参数必须进行UrlEncode之后才可以发送给支付宝
        /// </summary>
        [MaxLength(512)]
        public string PassbackParams { get; set; }

        /// <summary>
        /// 优惠参数
        /// 注：仅与支付宝协商后可用
        /// </summary>
        [MaxLength(512)]
        public string PromoParams { get; set; }

        //业务扩展参数
        //public string ExtendParams { get; set; }

        /// <summary>
        /// 商户门店编号。该参数用于请求参数中以区分各门店，非必传项。
        /// </summary>
        [MaxLength(32)]
        public string StoreId { get; set; }

        /// <summary>
        /// 支付宝服务器主动通知商户服务器里指定的页面http/https路径。建议商户使用https
        /// </summary>
        [MaxLength(256)]
        [Required]
        public string NotifyUrl { get; set; }

        /// <summary>
        /// 禁用渠道，用户不可用指定渠道支付
        /// 当有多个渠道时用“,”分隔
        /// 注：与enable_pay_channels互斥
        /// </summary>
        [MaxLength(128)]
        public string DisablePayChannels { get; set; }

        /// <summary>
        /// 可用渠道，用户只能在指定渠道范围内支付
        /// 当有多个渠道时用“,”分隔
        /// 注：与disable_pay_channels互斥
        /// </summary>
        [MaxLength(128)]
        public string EnablePayChannels { get; set; }
    }
}

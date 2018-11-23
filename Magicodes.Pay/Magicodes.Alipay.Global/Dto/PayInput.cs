// ======================================================================
//   
//           Copyright (C) 2018-2020 湖南心莱信息科技有限公司    
//           All rights reserved
//   
//           filename : PayInput.cs
//           description :
//   
//           created by 雪雁 at  2018-11-23 9:38
//           Mail: wenqiang.li@xin-lai.com
//           QQ群：85318032（技术交流）
//           Blog：http://www.cnblogs.com/codelove/
//           GitHub：https://github.com/xin-lai
//           Home：http://xin-lai.com
//   
// ======================================================================

using System;
using System.ComponentModel.DataAnnotations;

namespace Magicodes.Alipay.Global.Dto
{
    /// <summary>
    ///     Defines the <see cref="PayInput" />
    /// </summary>
    public class PayInput
    {
        /// <summary>
        ///     对一笔交易的具体描述信息。如果是多种商品，请将商品描述字符串累加传给body。
        /// </summary>
        [MaxLength(400)]
        public string Body { get; set; }

        /// <summary>
        ///     商品的标题/交易标题/订单标题/订单关键字等。
        /// </summary>
        [Required]
        [MaxLength(255)]
        public string Subject { get; set; }

        /// <summary>
        ///     商户网站唯一订单号（为空则自动生成）
        /// </summary>
        [MaxLength(64)]
        public string TradeNo { get; set; }

        /// <summary>
        /// 结算币种
        /// </summary>
        [MaxLength(10)]
        public string Currency { get; set; }

        /// <summary>
        ///     商品的外币金额，范围是0.01～1000000.00.
        /// </summary>
        [Range(0.01, 1000000)]
        public decimal TotalFee { get; set; }


        /// <summary>
        /// 范围为0.01～1000000.00,如果商户网站使用人民币进行标价就是用这个参数来替换total_fee参数，rmb_fee和total_fee不能同时使用
        /// </summary>
        [Range(0.01, 1000000)]
        public decimal RmbFee { get; set; }


        /// <summary>
        ///     默认12小时，最大15天。此为买家登陆到完成支付的有效时间
        /// </summary>
        [MaxLength(10)]
        public string TimeoutRule { get; set; }

        /// <summary>
        /// 快捷登录返回的安全令牌。快捷登录的需要传。
        /// </summary>
        [MaxLength(40)]
        public string AuthToken { get; set; }

        /// <summary>
        /// YYYY-MM-DD HH:MM:SS 这里请使用北京时间以便于和支付宝系统时间匹配，此参数必须要和order_valid_time参数一起使用，控制从跳转到买家登陆的有效时间
        /// </summary>
        public DateTime? OrderGmtCreate { get; set; }


        /// <summary>
        /// 最大值为2592000，单位为秒，此参数必须要和order_gmt_create参数一起使用，控制从跳转到买家登陆的有效时间	
        /// </summary>
        public int? OrderValidTime { get; set; }

        /// <summary>
        ///     显示供货商名字
        /// </summary>
        [MaxLength(16)]
        public string Supplier { get; set; }

        /// <summary>
        ///     由支付机构给二级商户分配的唯一ID	
        /// </summary>
        [MaxLength(64)]
        public string SecondaryMerchantId { get; set; }

        /// <summary>
        ///  由支付机构给二级商户分配的唯一名称
        /// </summary>
        [MaxLength(128)]
        public string SecondaryMerchantName { get; set; }

        /// <summary>
        ///     支付宝分配的二级商户的行业代码，如公共饮食行业、餐馆：5812,百货公司：5311,住房-旅馆：7011,出租车：4121。
        /// </summary>
        [MaxLength(4)]
        public string SecondaryMerchantIndustry { get; set; }

        /// <summary>
        ///     支付宝服务器主动通知商户服务器里指定的页面http/https路径。建议商户使用https
        /// </summary>
        [MaxLength(256)]
        [Required]
        public string NotifyUrl { get; set; }


        /// <summary>
        ///     页面跳转同步通知页面路径，需http://格式的完整路径，不能加?id=123这类自定义参数，必须外网可以正常访问
        /// </summary>
        [MaxLength(256)]
        public string ReturnUrl { get; set; }
    }
}
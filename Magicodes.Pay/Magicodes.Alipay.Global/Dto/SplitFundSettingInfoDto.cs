using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace Magicodes.Alipay.Global.Dto
{
    public class SplitFundSettingInfoDto
    {
        /// <summary>
        /// 接受分账资金的支付宝账户ID。 以2088开头的纯16位数字。
        /// </summary>
        [JsonProperty("transIn")]
        [Required]
        [MaxLength(16)]
        public string TransIn { get; set; }

        /// <summary>
        /// 分账比率
        /// </summary>
        [JsonProperty("amountRate")]
        [Required]
        [Range(0.1, 1)]
        public decimal AmountRate { get; set; }

        /// <summary>
        /// 分账描述信息。
        /// </summary>
        [JsonProperty("desc")]
        [MaxLength(200)]
        public string Desc { get; set; }
    }
}
using System;
using System.Net;
using System.Threading.Tasks;
using Magicodes.Allinpay.Dto;
using Newtonsoft.Json;

namespace Magicodes.Allinpay
{
    /// <summary>
    /// 通联支付服务
    /// </summary>
    public class AllinpayAppService: IAllinpayAppService
    {
        private readonly IAllinpaySettings _allinpaySettings;

        public AllinpayAppService()
        {
            _allinpaySettings = GetPayConfigFunc();
        }

        public static Action<string, string> LoggerAction { get; set; }
        public static Func<IAllinpaySettings> GetPayConfigFunc { get; set; }

        /// <summary>
        ///     APP支付
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public Task<WeChatMiniPayOutput> WeChatMiniPay(WeChatMiniPayInput input)
        {
            var allinpayDefaultClient = new AllinpayDefaultClient(_allinpaySettings.CusId, _allinpaySettings.AppId,_allinpaySettings.AppKey);
            var response = allinpayDefaultClient.WeChatMiniPay(input);
            if (response.RetCode== "FAIL")
            {
                LoggerAction?.Invoke("Error", "通联支付请求参数错误");
                throw new Exception("通联支付请求参数错误,请检查!");
            }

            return Task.FromResult(new WeChatMiniPayOutput
            {
                Response = response
            });
        }

        
    }
}

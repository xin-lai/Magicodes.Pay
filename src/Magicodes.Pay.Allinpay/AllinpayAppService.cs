using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Magicodes.Pay.Allinpay.Dto;

namespace Magicodes.Pay.Allinpay
{
    /// <summary>
    /// 通联支付服务
    /// </summary>
    public class AllinpayAppService : IAllinpayAppService
    {
        private IAllinpaySettings AllinpaySettings {get; set;}

        public AllinpayAppService()
        {
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
            AllinpaySettings = GetPayConfigFunc();
            var allinpayDefaultClient = new AllinpayDefaultClient(AllinpaySettings);
            var response = allinpayDefaultClient.WeChatMiniPay(input);
            if (response.RetCode == "FAIL")
            {
                LoggerAction?.Invoke("Error", "通联支付请求参数错误（FAIL）:" + AllinpaySettings);
                LoggerAction?.Invoke("Error", "input:" + input);
                LoggerAction?.Invoke("Error", "RetMsg:" + response.RetMsg + "    ErrMsg:" + response.ErrMsg);
                throw new Exception("通联支付请求参数错误,请检查!");
            }

            return Task.FromResult(new WeChatMiniPayOutput
            {
                Response = response
            });
        }

        /// <summary>
        ///     JSAPI支付
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public Task<JsApiPayOutput> WeChatJsApiPay(JsApiPayInput input)
        {
            AllinpaySettings = GetPayConfigFunc();
            var allinpayDefaultClient = new AllinpayDefaultClient(AllinpaySettings);
            var response = allinpayDefaultClient.WeChatJsApiPay(input);
            if (response.RetCode == "FAIL")
            {
                LoggerAction?.Invoke("Error", "通联支付请求参数错误（FAIL）:" + AllinpaySettings);
                LoggerAction?.Invoke("Error", "input:" + input);
                LoggerAction?.Invoke("Error", "RetMsg:" + response.RetMsg + "    ErrMsg:" + response.ErrMsg);
                throw new Exception("通联支付请求参数错误,请检查!");
            }

            return Task.FromResult(new JsApiPayOutput
            {
                Response = response
            });
        }

        /// <summary>
        ///     支付回调通知处理
        /// </summary>
        /// <param name="dic"></param>
        /// <returns></returns>
        public bool PayNotifyHandler(Dictionary<string, string> dic)
        {
            try
            {
                AllinpaySettings = GetPayConfigFunc();
                var allinpayKey = AllinpaySettings.AppKey;
                if (!dic.ContainsKey("sign"))//如果不包含sign,则不进行处理
                {
                    LoggerAction?.Invoke("Error", "sign is null");
                    return false;
                }
                return AllinpayUtil.ValidSign(dic, allinpayKey);
            }
            catch (Exception e)
            {
                LoggerAction?.Invoke("Error", e.Message);
                return false;
            }
        }
    }
}

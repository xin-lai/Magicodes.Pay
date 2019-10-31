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
            var allinpayDefaultClient = new AllinpayDefaultClient(_allinpaySettings);
            var response = allinpayDefaultClient.WeChatMiniPay(input);
            if (response.RetCode == "FAIL")
            {
                LoggerAction?.Invoke("Error", "通联支付请求参数错误（FAIL）:" + _allinpaySettings);
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
        ///     支付回调通知处理
        /// </summary>
        /// <param name="dic"></param>
        /// <returns></returns>
        public bool PayNotifyHandler(Dictionary<string, string> dic)
        {
            try
            {
                var allinpayKey = _allinpaySettings.AppKey;
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

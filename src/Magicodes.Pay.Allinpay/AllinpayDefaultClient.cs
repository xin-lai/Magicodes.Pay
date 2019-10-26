using System;
using System.Collections.Generic;
using Magicodes.Pay.Allinpay.Dto;
using Newtonsoft.Json;

namespace Magicodes.Pay.Allinpay
{
    public class AllinpayDefaultClient
    {
        private readonly IAllinpaySettings _allinpaySettings;


        public AllinpayDefaultClient(IAllinpaySettings allinpaySettings)
        {
            _allinpaySettings = allinpaySettings;
        }

        public AllinpayResponse WeChatMiniPay(WeChatMiniPayInput input)
        {
            var paramDic = BuildBasicParam();
            paramDic.Add("trxamt", input.Amount.ToString());
            paramDic.Add("reqsn", input.OrderNumber);
            paramDic.Add("paytype", "W06");
            paramDic.Add("body", input.Body);
            paramDic.Add("remark", input.Remark);
            paramDic.Add("acct", input.OpenId);
            paramDic.Add("sub_appid", _allinpaySettings.WeChatAppId);
            paramDic.Add("notify_url", _allinpaySettings.NotifyUrl);
            paramDic.Add("validtime", input.ValidTime);
            paramDic.Add("sign", AllinpayUtil.SignParam(paramDic, _allinpaySettings.AppKey));
            var result = HttpRequestUtil.PostAsync($"{_allinpaySettings.ApiGateWay}/pay", paramDic).Result;
            var response = JsonConvert.DeserializeObject<AllinpayResponse>(result);
            return response;
        }

        private Dictionary<string, string> BuildBasicParam()
        {
            var paramDic = new Dictionary<string, string>
            {
                {"cusid", _allinpaySettings.CusId},
                {"appid", _allinpaySettings.AppId},
                {"version", _allinpaySettings.Version},
                {"randomstr", DateTime.Now.ToFileTime().ToString()}
            };
            return paramDic;
        }
    }
}
using System;
using System.Collections.Generic;
using Magicodes.Allinpay.Dto;
using Newtonsoft.Json;

namespace Magicodes.Allinpay
{
    public class AllinpayDefaultClient
    {
        /// <summary>
        ///     接口网关
        /// </summary>
        private readonly string _apiGateWay;

        /// <summary>
        ///     平台分配的APPID
        /// </summary>
        private readonly string _appId;

        /// <summary>
        ///     平台分配的AppKey
        /// </summary>
        private readonly string _appKey;

        /// <summary>
        ///     实际交易的商户号
        /// </summary>
        private readonly string _cusId;

        /// <summary>
        ///     版本
        /// </summary>
        private readonly string _version;

        public AllinpayDefaultClient(string cusId, string appId, string appKey,
            string apiGateWay = "https://vsp.allinpay.com/apiweb/unitorder", string version = "11")
        {
            _cusId = cusId;
            _appId = appId;
            _appKey = appKey;
            _version = version;
            _apiGateWay = apiGateWay;
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
            paramDic.Add("sub_appid", input.WeChatAppId);
            paramDic.Add("notify_url", input.NotifyUrl);
            paramDic.Add("validtime", input.ValidTime);
            paramDic.Add("sign", AllinpayUtil.SignParam(paramDic, _appKey));
            var result = HttpRequestUtil.PostAsync($"{_apiGateWay}/pay", paramDic).Result;
            var response = JsonConvert.DeserializeObject<AllinpayResponse>(result);
            return response;
        }

        private Dictionary<string, string> BuildBasicParam()
        {
            var paramDic = new Dictionary<string, string>
            {
                {"cusid", _cusId},
                {"appid", _appId},
                {"version", _version},
                {"randomstr", DateTime.Now.ToFileTime().ToString()}
            };
            return paramDic;
        }
    }
}
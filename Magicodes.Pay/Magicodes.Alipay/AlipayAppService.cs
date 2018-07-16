using Alipay.AopSdk.Core;
using Alipay.AopSdk.Core.Domain;
using Alipay.AopSdk.Core.Request;
using Alipay.AopSdk.Core.Response;
using Magicodes.Alipay.Dto;
using System;
using System.Globalization;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Magicodes.Alipay
{
    /// <summary>
    /// 支付宝支付
    /// </summary>
    public class AlipayAppService : IAlipayAppService
    {
        private readonly IAlipaySettings alipaySettings;

        public AlipayAppService()
        {
            alipaySettings = GetPayConfigFunc();
        }

        public static Action<string, string> LoggerAction { get; set; }
        public static Func<IAlipaySettings> GetPayConfigFunc { get; set; }

        /// <summary>
        /// APP支付
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public Task<AppPayOutput> AppPay(AppPayInput input)
        {
            var client = GetClient();

            var request = new AlipayTradeAppPayRequest();
            var model = new AlipayTradeAppPayModel()
            {
                Body = input.Body,
                DisablePayChannels = input.DisablePayChannels,
                EnablePayChannels = input.EnablePayChannels,
                //ExtendParams = input.ex
                GoodsType = input.GoodsType,
                OutTradeNo = input.TradeNo ?? Guid.NewGuid().ToString("N"),
                PassbackParams = WebUtility.UrlEncode(input.PassbackParams),
                ProductCode = "QUICK_MSECURITY_PAY",
                PromoParams = input.PromoParams,
                //SpecifiedChannel
                StoreId = input.StoreId,
                Subject = input.Subject,
                TimeoutExpress = input.TimeoutExpress,
                TotalAmount = input.TotalAmount.ToString(),
            };
            request.SetBizModel(model);
            request.SetNotifyUrl(input.NotifyUrl ?? alipaySettings.Notify);
            var response = client.SdkExecute(request);
            if (response.IsError)
            {
                LoggerAction?.Invoke("Error", "支付宝支付请求参数错误：" + JsonConvert.SerializeObject(model));
                throw new AlipayExcetion("支付宝支付请求参数错误,请检查!");
            }
            return Task.FromResult(new AppPayOutput()
            {
                Response = response
            });
        }

        /// <summary>
        /// Wap支付
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public Task<WapPayOutput> WapPay(WapPayInput input)
        {
            var client = GetClient();

            var request = new AlipayTradeWapPayRequest();
            var model = new AlipayTradeWapPayModel()
            {
                Body = input.Body,
                DisablePayChannels = input.DisablePayChannels,
                EnablePayChannels = input.EnablePayChannels,
                //ExtendParams = input.ex
                GoodsType = input.GoodsType,
                OutTradeNo = input.TradeNo ?? Guid.NewGuid().ToString("N"),
                PassbackParams = WebUtility.UrlEncode(input.PassbackParams),
                ProductCode = "QUICK_WAP_WAY",
                PromoParams = input.PromoParams,
                //SpecifiedChannel
                StoreId = input.StoreId,
                Subject = input.Subject,
                TimeoutExpress = input.TimeoutExpress,
                TotalAmount = input.TotalAmount.ToString(CultureInfo.InvariantCulture),
                QuitUrl = input.QuitUrl
            };
            request.SetBizModel(model);
            request.SetNotifyUrl(input.NotifyUrl ?? alipaySettings.Notify);
            var response = client.pageExecute<AlipayTradeWapPayResponse>(request);
            if (response.IsError)
            {
                LoggerAction?.Invoke("Error", "支付宝支付请求参数错误：" + JsonConvert.SerializeObject(model));
                throw new AlipayExcetion("支付宝支付请求参数错误,请检查!");
            }
            return Task.FromResult(new WapPayOutput()
            {
                Body = response.Body
            });
        }

        private DefaultAopClient GetClient()
        {
            try
            {
                var client =
                     new DefaultAopClient(
                        serverUrl: alipaySettings.Gatewayurl,
                        appId: alipaySettings.AppId,
                        privateKeyPem: alipaySettings.PrivateKey,
                        format: "json",
                        version: "1.0",
                        signType: alipaySettings.SignType,
                        alipayPulicKey: alipaySettings.AlipayPublicKey,
                        charset: alipaySettings.CharSet,
                        keyFromFile: alipaySettings.IsKeyFromFile);
                return client;
            }
            catch (Exception ex)
            {
                LoggerAction?.Invoke("Error", "支付宝配置错误：");
                throw new AlipayExcetion("支付宝配置错误,请检查!", ex);
            }
        }
    }
}

// ======================================================================
//   
//           Copyright (C) 2018-2020 湖南心莱信息科技有限公司    
//           All rights reserved
//   
//           filename : AlipayAppService.cs
//           description :
//   
//           created by 雪雁 at  2018-07-16 15:42
//           Mail: wenqiang.li@xin-lai.com
//           QQ群：85318032（技术交流）
//           Blog：http://www.cnblogs.com/codelove/
//           GitHub：https://github.com/xin-lai
//           Home：http://xin-lai.com
//   
// ======================================================================

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Threading.Tasks;
using Alipay.AopSdk.Core;
using Alipay.AopSdk.Core.Domain;
using Alipay.AopSdk.Core.Request;
using Alipay.AopSdk.Core.Util;
using Magicodes.Alipay.Dto;
using Newtonsoft.Json;

namespace Magicodes.Alipay
{
    /// <summary>
    ///     支付宝支付
    /// </summary>
    public class AlipayAppService : IAlipayAppService
    {
        private readonly IAlipaySettings _alipaySettings;

        public AlipayAppService()
        {
            _alipaySettings = GetPayConfigFunc();
        }

        public static Action<string, string> LoggerAction { get; set; }
        public static Func<IAlipaySettings> GetPayConfigFunc { get; set; }

        /// <summary>
        ///     APP支付
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public Task<AppPayOutput> AppPay(AppPayInput input)
        {
            var client = GetClient();

            var request = new AlipayTradeAppPayRequest();
            var model = new AlipayTradeAppPayModel
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
                TotalAmount = input.TotalAmount.ToString()
            };
            request.SetBizModel(model);
            request.SetNotifyUrl(input.NotifyUrl ?? _alipaySettings.Notify);
            var response = client.SdkExecute(request);
            if (response.IsError)
            {
                LoggerAction?.Invoke("Error", "支付宝支付请求参数错误：" + JsonConvert.SerializeObject(model));
                throw new AlipayExcetion("支付宝支付请求参数错误,请检查!");
            }

            return Task.FromResult(new AppPayOutput
            {
                Response = response
            });
        }

        /// <summary>
        ///     Wap支付
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public Task<WapPayOutput> WapPay(WapPayInput input)
        {
            var client = GetClient();

            var request = new AlipayTradeWapPayRequest();
            var model = new AlipayTradeWapPayModel
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
            request.SetNotifyUrl(input.NotifyUrl ?? _alipaySettings.Notify);
            var response = client.pageExecute(request);
            if (response.IsError)
            {
                LoggerAction?.Invoke("Error", "支付宝支付请求参数错误：" + JsonConvert.SerializeObject(model));
                throw new AlipayExcetion("支付宝支付请求参数错误,请检查!");
            }

            return Task.FromResult(new WapPayOutput
            {
                Body = response.Body
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
                var alipaySignPublicKey = _alipaySettings.AlipaySignPublicKey;
                var charset = _alipaySettings.CharSet;
                var signtype = _alipaySettings.SignType;
                var flag = AlipaySignature.RSACheckV1(dic, alipaySignPublicKey, charset, signtype, false);
                return flag;
            }
            catch (Exception e)
            {
                LoggerAction?.Invoke("Error", e.Message);
                return false;
            }
        }


        private DefaultAopClient GetClient()
        {
            try
            {
                var client =
                    new DefaultAopClient(
                        _alipaySettings.GatewayUrl,
                        _alipaySettings.AppId,
                        _alipaySettings.PrivateKey,
                        "json",
                        "1.0",
                        _alipaySettings.SignType,
                        _alipaySettings.AlipayPublicKey,
                        _alipaySettings.CharSet,
                        _alipaySettings.IsKeyFromFile);
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
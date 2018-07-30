using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Xml.Serialization;
using Magicodes.Pay.WeChat.Config;
using Magicodes.Pay.WeChat.Helper;
using Magicodes.Pay.WeChat.Pay.Dto;
using Magicodes.Pay.WeChat.Pay.Models;

namespace Magicodes.Pay.WeChat
{
    /// <summary>
    /// 小程序支付接口
    /// </summary>
    public class WeChatPayApi
    {
        private WeChatPayHelper weChatPayHelper = new WeChatPayHelper();

        /// <summary>
        /// 小程序支付
        /// 小程序统一下单接口
        /// https://pay.weixin.qq.com/wiki/doc/api/wxa/wxa_api.php?chapter=9_1
        /// </summary>
        /// <returns></returns>
        public MiniProgramPayOutput MiniProgramPay(MiniProgramPayInput input)
        {
            if (input == null)
            {
                throw new ArgumentNullException(nameof(input));
            }
            if (input.TotalFee <= 0)
            {
                throw new ArgumentException("金额不能小于0!", nameof(input.TotalFee));
            }
            if (string.IsNullOrWhiteSpace(input.OpenId))
            {
                throw new ArgumentNullException("OpenId必须填写!", nameof(input.OpenId));
            }

            if (string.IsNullOrWhiteSpace(input.Body))
            {
                throw new ArgumentNullException("商品描述必须填写!", nameof(input.Body));
            }

            if (string.IsNullOrWhiteSpace(input.SpbillCreateIp))
            {
                throw new ArgumentNullException("终端IP必须填写!", nameof(input.SpbillCreateIp));
            }

            var url = "https://api.mch.weixin.qq.com/pay/unifiedorder";

            var config = WeChatPayHelper.GetPayConfigFunc();
            if (config == null)
            {
                throw new ArgumentNullException("请配置支付配置信息!", "PayConfig");
            }
            if (string.IsNullOrWhiteSpace(config.PayAppId))
            {
                throw new ArgumentNullException("PayAppId必须配置!", nameof(config.PayAppId));
            }
            if (string.IsNullOrWhiteSpace(config.MchId))
            {
                throw new ArgumentNullException("MchId(商户Id)必须配置!", nameof(config.MchId));
            }
            if (string.IsNullOrWhiteSpace(config.PayNotifyUrl))
            {
                throw new ArgumentNullException("PayNotifyUrl(支付回调地址)必须配置!", nameof(config.PayNotifyUrl));
            }
            if (string.IsNullOrWhiteSpace(config.TenPayKey))
            {
                throw new ArgumentNullException("TenPayKey(支付密钥)必须配置!", nameof(config.TenPayKey));
            }

            var model = new UnifiedorderRequest
            {
                AppId = config.PayAppId,
                MchId = config.MchId,
                Attach = input.Attach,
                Body = input.Body,
                Detail = input.Detail,
                DeviceInfo = input.DeviceInfo,
                FeeType = input.FeeType,
                GoodsTag = input.GoodsTag,
                LimitPay = input.LimitPay,
                OpenId = input.OpenId,
                OutTradeNo = input.OutTradeNo ?? weChatPayHelper.GenerateOutTradeNo(),
                SpbillCreateIp = input.SpbillCreateIp,
                TimeExpire = input.TimeExpire,
                TimeStart = input.TimeStart,
                TotalFee = ((int)(input.TotalFee * 100)).ToString(),
                NonceStr = weChatPayHelper.GetNoncestr(),
                NotifyUrl = config.PayNotifyUrl,
            };
            var dictionary = weChatPayHelper.GetDictionaryByType(model);
            model.Sign = weChatPayHelper.CreateMd5Sign(dictionary, config.TenPayKey); //生成Sign

            var result = weChatPayHelper.PostXML<UnifiedorderResult>(url, model);
            if (result.IsSuccess())
            {
                var data = new
                {
                    appId = result.AppId,
                    nonceStr = result.NonceStr,
                    package = "prepay_id=" + result.PrepayId,
                    signType = "MD5",
                    timeStamp = weChatPayHelper.GetTimestamp(),
                };
                return new MiniProgramPayOutput()
                {
                    AppId = data.appId,
                    Package = data.package,
                    NonceStr = data.nonceStr,
                    PaySign = weChatPayHelper.CreateMd5Sign(weChatPayHelper.GetDictionaryByType(data), config.TenPayKey),
                    SignType = data.signType,
                    TimeStamp = data.timeStamp
                };
            }
            WeChatPayHelper.LoggerAction("Error", "支付错误：" + result.GetFriendlyMessage());
            throw new WeChatPayException("支付错误，请联系客服或重新支付！");
        }

        /// <summary>
        /// APP支付
        /// APP统一下单接口
        /// https://pay.weixin.qq.com/wiki/doc/api/app/app.php?chapter=9_1
        /// </summary>
        /// <returns></returns>
        public AppPayOutput AppPay(AppPayInput input)
        {
            if (input == null)
            {
                throw new ArgumentNullException(nameof(input));
            }
            if (input.TotalFee <= 0)
            {
                throw new ArgumentException("金额不能小于0!", nameof(input.TotalFee));
            }
            if (string.IsNullOrWhiteSpace(input.Body))
            {
                throw new ArgumentNullException("商品描述必须填写!", nameof(input.Body));
            }

            if (string.IsNullOrWhiteSpace(input.SpbillCreateIp))
            {
                throw new ArgumentNullException("终端IP必须填写!", nameof(input.SpbillCreateIp));
            }

            var url = "https://api.mch.weixin.qq.com/pay/unifiedorder";

            var config = WeChatPayHelper.GetPayConfigFunc();
            if (config == null)
            {
                throw new ArgumentNullException("请配置支付配置信息!", "PayConfig");
            }
            if (string.IsNullOrWhiteSpace(config.PayAppId))
            {
                throw new ArgumentNullException("PayAppId必须配置!", nameof(config.PayAppId));
            }
            if (string.IsNullOrWhiteSpace(config.MchId))
            {
                throw new ArgumentNullException("MchId(商户Id)必须配置!", nameof(config.MchId));
            }
            if (string.IsNullOrWhiteSpace(config.PayNotifyUrl))
            {
                throw new ArgumentNullException("PayNotifyUrl(支付回调地址)必须配置!", nameof(config.PayNotifyUrl));
            }
            if (string.IsNullOrWhiteSpace(config.TenPayKey))
            {
                throw new ArgumentNullException("TenPayKey(支付密钥)必须配置!", nameof(config.TenPayKey));
            }

            var model = new AppUnifiedorderRequest
            {
                AppId = config.PayAppId,
                MchId = config.MchId,
                Attach = input.Attach,
                Body = input.Body,
                Detail = input.Detail,
                FeeType = input.FeeType,
                GoodsTag = input.GoodsTag,
                LimitPay = input.LimitPay,
                OutTradeNo = input.OutTradeNo ?? weChatPayHelper.GenerateOutTradeNo(),
                SpbillCreateIp = input.SpbillCreateIp,
                TimeExpire = input.TimeExpire,
                TimeStart = input.TimeStart,
                TotalFee = ((int)(input.TotalFee * 100)).ToString(),
            };
            model.NonceStr = weChatPayHelper.GetNoncestr();
            model.NotifyUrl = config.PayNotifyUrl;
            var dictionary = weChatPayHelper.GetDictionaryByType(model);
            model.Sign = weChatPayHelper.CreateMd5Sign(dictionary, config.TenPayKey); //生成Sign

            var result = weChatPayHelper.PostXML<UnifiedorderResult>(url, model);
            if (result.IsSuccess())
            {
                var data = new
                {
                    appId = result.AppId,
                    nonceStr = result.NonceStr,
                    package = "prepay_id=" + result.PrepayId,
                    signType = "MD5",
                    timeStamp = weChatPayHelper.GetTimestamp(),
                };
                return new AppPayOutput()
                {
                    AppId = data.appId,
                    MchId = model.MchId,
                    PrepayId = result.PrepayId,
                    NonceStr = data.nonceStr,
                    PaySign = model.Sign,
                    SignType = data.signType,
                    TimeStamp = data.timeStamp
                };
            }
            WeChatPayHelper.LoggerAction("Error", "支付错误：" + result.GetFriendlyMessage());
            throw new WeChatPayException("支付错误，请联系客服或重新支付！");
        }

        /// <summary>
        /// 支付回调通知处理
        /// </summary>
        /// <param name="inputStream"></param>
        /// <returns></returns>
        public PayNotifyOutput PayNotifyHandler(Stream inputStream)
        {
            PayNotifyOutput result = null;
            var data = weChatPayHelper.PostInput(inputStream);
            try
            {
                result = XmlHelper.DeserializeObject<PayNotifyOutput>(data);
            }
            catch (Exception ex)
            {
                WeChatPayHelper.LoggerAction?.Invoke("Error", "解析支付回调参数出错：" + data + "  Exception:" + ex.ToString());
                throw;
            }
            return result;
        }


    }
}

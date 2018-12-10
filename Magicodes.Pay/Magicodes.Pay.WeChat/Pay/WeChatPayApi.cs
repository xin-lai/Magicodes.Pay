// ======================================================================
//   
//           Copyright (C) 2018-2020 湖南心莱信息科技有限公司    
//           All rights reserved
//   
//           filename : WeChatPayApi.cs
//           description :
//   
//           created by 雪雁 at  2018-07-16 15:46
//           Mail: wenqiang.li@xin-lai.com
//           QQ群：85318032（技术交流）
//           Blog：http://www.cnblogs.com/codelove/
//           GitHub：https://github.com/xin-lai
//           Home：http://xin-lai.com
//   
// ======================================================================

using Magicodes.Pay.WeChat.Config;
using Magicodes.Pay.WeChat.Helper;
using Magicodes.Pay.WeChat.Pay.Dto;
using Magicodes.Pay.WeChat.Pay.Models;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Magicodes.Pay.WeChat
{
    /// <summary>
    ///     微信支付接口
    /// </summary>
    public class WeChatPayApi
    {
        private const string Fail_Xml_Tpl =
            "<xml><return_code><![CDATA[FAIL]]></return_code><return_msg><![CDATA[{0}]]></return_msg></xml>";

        private readonly WeChatPayHelper weChatPayHelper = new WeChatPayHelper();

        /// <summary>
        ///     小程序支付
        ///     小程序统一下单接口
        ///     https://pay.weixin.qq.com/wiki/doc/api/wxa/wxa_api.php?chapter=9_1
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
            var config = GetConfig();

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
                NotifyUrl = config.PayNotifyUrl
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
                    timeStamp = weChatPayHelper.GetTimestamp()
                };
                return new MiniProgramPayOutput
                {
                    AppId = data.appId,
                    Package = data.package,
                    NonceStr = data.nonceStr,
                    PaySign =
                        weChatPayHelper.CreateMd5Sign(weChatPayHelper.GetDictionaryByType(data), config.TenPayKey),
                    SignType = data.signType,
                    TimeStamp = data.timeStamp
                };
            }

            WeChatPayHelper.LoggerAction("Error", "支付错误：" + result.GetFriendlyMessage());
            throw new WeChatPayException("支付错误，请联系客服或重新支付！");
        }

        private static IWeChatPayConfig GetConfig()
        {
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

            return config;
        }

        /// <summary>
        ///     APP支付
        ///     APP统一下单接口
        ///     https://pay.weixin.qq.com/wiki/doc/api/app/app.php?chapter=9_1
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

            var config = GetConfig();

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
                NonceStr = weChatPayHelper.GetNoncestr(),
                NotifyUrl = config.PayNotifyUrl
            };
            model.Sign = CreateSign(model);

            var result = weChatPayHelper.PostXML<UnifiedorderResult>(url, model);
            if (result.IsSuccess())
            {
                var data = new
                {
                    appid = result.AppId,
                    noncestr = result.NonceStr,
                    partnerid = result.Mch_Id,
                    prepayid = result.PrepayId,
                    package = "Sign=WXPay",
                    timestamp = weChatPayHelper.GetTimestamp()
                };
                return new AppPayOutput
                {
                    AppId = data.appid,
                    MchId = model.MchId,
                    PrepayId = result.PrepayId,
                    NonceStr = data.noncestr,
                    PaySign =
                        weChatPayHelper.CreateMd5Sign(weChatPayHelper.GetDictionaryByType(data), config.TenPayKey),
                    SignType = "MD5",
                    TimeStamp = data.timestamp,
                    Package = data.package
                };
            }

            WeChatPayHelper.LoggerAction("Error", "支付错误：" + result.GetFriendlyMessage());
            throw new WeChatPayException("支付错误，请联系客服或重新支付！");
        }

        private string CreateSign<T>(T model)
        {
            var config = GetConfig();
            var dictionary = weChatPayHelper.GetDictionaryByType(model);
            return weChatPayHelper.CreateMd5Sign(dictionary, config.TenPayKey); //生成Sign
        }

        /// <summary>
        ///     支付回调通知处理
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
                WeChatPayHelper.LoggerAction?.Invoke("Error", "解析支付回调参数出错：" + data + "  Exception:" + ex);
                throw;
            }

            return result;
        }

        /// <summary>
        ///     处理支付回调参数
        /// </summary>
        /// <param name="inputStream">输入流</param>
        /// <param name="payHandlerFunc">支付处理逻辑函数</param>
        /// <returns>处理结果</returns>
        public Task<string> PayNotifyHandler(Stream inputStream, Action<PayNotifyOutput, string> payHandlerFunc)
        {
            PayNotifyOutput result = null;
            var data = weChatPayHelper.PostInput(inputStream);
            var outPutXml = string.Empty;
            var error = string.Empty;
            try
            {
                result = XmlHelper.DeserializeObject<PayNotifyOutput>(data);
            }
            catch (Exception ex)
            {
                WeChatPayHelper.LoggerAction?.Invoke("Error", "解析支付回调参数出错：" + data + "  Exception:" + ex);
                outPutXml = string.Format(Fail_Xml_Tpl, "解析支付回调参数出错");
                error = ex.ToString();
            }

            if (!string.IsNullOrWhiteSpace(outPutXml))
            {
            }
            else if (string.IsNullOrWhiteSpace(result?.TransactionId))
            {
                error = "支付结果中微信订单号不存在";
                outPutXml = string.Format(Fail_Xml_Tpl, error);
            }
            else if (!result.IsSuccess())
            {
                outPutXml = string.Format(Fail_Xml_Tpl, "回调处理失败");
                error = $"回调处理失败：ErrCode:{result.ErrCode} \nErrCodeDes:{result.ErrCodeDes}";
            }
            //查询订单，判断订单真实性
            else if (!QueryOrder(result.TransactionId))
            {
                error = "订单不存在";
                outPutXml = string.Format(Fail_Xml_Tpl, error);
            }

            payHandlerFunc?.Invoke(result, error);

            return Task.FromResult(!string.IsNullOrWhiteSpace(outPutXml)
                ? outPutXml
                : "<xml><return_code><![CDATA[SUCCESS]]></return_code></xml>");
        }

        /// <summary>
        ///     订单查询
        ///     该接口提供所有微信支付订单的查询，商户可以通过查询订单接口主动查询订单状态，完成下一步的业务逻辑。
        ///     需要调用查询接口的情况：
        ///     ◆ 当商户后台、网络、服务器等出现异常，商户系统最终未接收到支付通知；
        ///     ◆ 调用支付接口后，返回系统错误或未知交易状态情况；
        ///     ◆ 调用刷卡支付API，返回USERPAYING的状态；
        ///     ◆ 调用关单或撤销接口API之前，需确认支付状态；
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public OrderQueryOutput OrderQuery(OrderQueryInput input)
        {
            var url = "https://api.mch.weixin.qq.com/pay/orderquery";
            //检测必填参数
            if (string.IsNullOrWhiteSpace(input.TransactionId) && string.IsNullOrWhiteSpace(input.OutTradeNo))
            {
                throw new WeChatPayException("订单查询接口中，out_trade_no、transaction_id至少填一个！");
            }

            var config = GetConfig();
            var req = new OrderQueryRequest
            {
                AppId = config.PayAppId,
                MchId = config.MchId,
                NonceStr = weChatPayHelper.GetNoncestr(),
                OutTradeNo = input.OutTradeNo,
                TransactionId = input.TransactionId,
                SignType = "MD5"
            };
            req.Sign = CreateSign(req);

            return weChatPayHelper.PostXML<OrderQueryOutput>(url, req);
        }

        /// <summary>
        ///     查询订单是否存在
        /// </summary>
        /// <param name="transactionId"></param>
        /// <returns></returns>
        private bool QueryOrder(string transactionId) => OrderQuery(new OrderQueryInput
        {
            TransactionId = transactionId
        }).IsSuccess();
    }
}
// ======================================================================
//   
//           Copyright (C) 2019-2020 湖南心莱信息科技有限公司    
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

using System;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Magicodes.Pay.Wxpay.Config;
using Magicodes.Pay.Wxpay.Helper;
using Magicodes.Pay.Wxpay.Pay.Dto;
using Magicodes.Pay.Wxpay.Pay.Models;
using Newtonsoft.Json;

namespace Magicodes.Pay.Wxpay.Pay
{
    /// <summary>
    ///     微信支付接口
    /// </summary>
    public class WeChatPayApi
    {
        private const string FailXmlTpl =
            "<xml><return_code><![CDATA[FAIL]]></return_code><return_msg><![CDATA[{0}]]></return_msg></xml>";

        private readonly WeChatPayHelper _weChatPayHelper = new WeChatPayHelper();

        #region 小程序统一下单接口

        /// <summary>
        ///     小程序支付
        ///     小程序统一下单接口
        ///     https://pay.weixin.qq.com/wiki/doc/api/wxa/wxa_api.php?chapter=9_1
        /// </summary>
        /// <returns></returns>
        public MiniProgramPayOutput MiniProgramPay(MiniProgramPayInput input)
        {
            if (input == null) throw new ArgumentNullException(nameof(input));

            if (input.TotalFee <= 0) throw new ArgumentException("金额不能小于0!", nameof(input.TotalFee));

            if (string.IsNullOrWhiteSpace(input.OpenId))
                throw new ArgumentNullException("OpenId必须填写!", nameof(input.OpenId));

            if (string.IsNullOrWhiteSpace(input.Body)) throw new ArgumentNullException("商品描述必须填写!", nameof(input.Body));

            if (string.IsNullOrWhiteSpace(input.SpbillCreateIp))
                throw new ArgumentNullException("终端IP必须填写!", nameof(input.SpbillCreateIp));

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
                OutTradeNo = input.OutTradeNo ?? _weChatPayHelper.GenerateOutTradeNo(),
                SpbillCreateIp = input.SpbillCreateIp,
                TimeExpire = input.TimeExpire,
                TimeStart = input.TimeStart,
                TotalFee = ((int) (input.TotalFee * 100)).ToString(),
                NonceStr = _weChatPayHelper.GetNoncestr(),
                NotifyUrl = config.PayNotifyUrl
            };
            var dictionary = _weChatPayHelper.GetDictionaryByType(model);
            model.Sign = _weChatPayHelper.CreateMd5Sign(dictionary, config.TenPayKey); //生成Sign

            var result = _weChatPayHelper.PostXML<UnifiedorderResult>(url, model);
            if (result.IsSuccess())
            {
                var data = new
                {
                    appId = result.AppId,
                    nonceStr = result.NonceStr,
                    package = "prepay_id=" + result.PrepayId,
                    signType = "MD5",
                    timeStamp = _weChatPayHelper.GetTimestamp()
                };
                return new MiniProgramPayOutput
                {
                    AppId = data.appId,
                    Package = data.package,
                    NonceStr = data.nonceStr,
                    PaySign =
                        _weChatPayHelper.CreateMd5Sign(_weChatPayHelper.GetDictionaryByType(data), config.TenPayKey),
                    SignType = data.signType,
                    TimeStamp = data.timeStamp
                };
            }

            WeChatPayHelper.LoggerAction("Error", "支付错误：" + result.GetFriendlyMessage());
            throw new WeChatPayException("支付错误，请联系客服或重新支付！");
        }

        #endregion

        #region  获取支付配置

        /// <summary>
        ///     获取支付配置
        /// </summary>
        /// <returns></returns>
        private static IWeChatPayConfig GetConfig()
        {
            var config = WeChatPayHelper.GetPayConfigFunc();
            if (config == null) throw new ArgumentNullException("请配置支付配置信息!", "PayConfig");

            if (string.IsNullOrWhiteSpace(config.PayAppId))
                throw new ArgumentNullException("PayAppId必须配置!", nameof(config.PayAppId));

            if (string.IsNullOrWhiteSpace(config.MchId))
                throw new ArgumentNullException("MchId(商户Id)必须配置!", nameof(config.MchId));

            if (string.IsNullOrWhiteSpace(config.PayNotifyUrl))
                throw new ArgumentNullException("PayNotifyUrl(支付回调地址)必须配置!", nameof(config.PayNotifyUrl));

            if (string.IsNullOrWhiteSpace(config.TenPayKey))
                throw new ArgumentNullException("TenPayKey(支付密钥)必须配置!", nameof(config.TenPayKey));

            return config;
        }

        #endregion

        #region APP统一下单接口

        /// <summary>
        ///     APP支付
        ///     APP统一下单接口
        ///     https://pay.weixin.qq.com/wiki/doc/api/app/app.php?chapter=9_1
        /// </summary>
        /// <returns></returns>
        public AppPayOutput AppPay(AppPayInput input)
        {
            if (input == null) throw new ArgumentNullException(nameof(input));

            if (input.TotalFee <= 0) throw new ArgumentException("金额不能小于0!", nameof(input.TotalFee));

            if (string.IsNullOrWhiteSpace(input.Body)) throw new ArgumentNullException("商品描述必须填写!", nameof(input.Body));

            if (string.IsNullOrWhiteSpace(input.SpbillCreateIp))
                throw new ArgumentNullException("终端IP必须填写!", nameof(input.SpbillCreateIp));

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
                OutTradeNo = input.OutTradeNo ?? _weChatPayHelper.GenerateOutTradeNo(),
                SpbillCreateIp = input.SpbillCreateIp,
                TimeExpire = input.TimeExpire,
                TimeStart = input.TimeStart,
                TotalFee = ((int) (input.TotalFee * 100)).ToString(),
                NonceStr = _weChatPayHelper.GetNoncestr(),
                NotifyUrl = config.PayNotifyUrl
            };
            model.Sign = CreateSign(model);

            var result = _weChatPayHelper.PostXML<UnifiedorderResult>(url, model);
            if (result.IsSuccess())
            {
                var data = new
                {
                    appid = result.AppId,
                    noncestr = result.NonceStr,
                    partnerid = result.Mch_Id,
                    prepayid = result.PrepayId,
                    package = "Sign=WXPay",
                    timestamp = _weChatPayHelper.GetTimestamp()
                };
                return new AppPayOutput
                {
                    AppId = data.appid,
                    MchId = model.MchId,
                    PrepayId = result.PrepayId,
                    NonceStr = data.noncestr,
                    PaySign =
                        _weChatPayHelper.CreateMd5Sign(_weChatPayHelper.GetDictionaryByType(data), config.TenPayKey),
                    SignType = "MD5",
                    TimeStamp = data.timestamp,
                    Package = data.package
                };
            }

            WeChatPayHelper.LoggerAction("Error", "支付错误：" + result.GetFriendlyMessage());
            throw new WeChatPayException("支付错误，请联系客服或重新支付！");
        }

        #endregion

        #region Native支付

        /// <summary>
        ///     Native支付
        ///     Native统一下单接口
        ///     https://pay.weixin.qq.com/wiki/doc/api/native.php?chapter=9_1
        /// </summary>
        /// <returns></returns>
        public NativePayOutput NativePay(NativePayInput input)
        {
            if (input == null) throw new ArgumentNullException(nameof(input));

            if (input.TotalFee <= 0) throw new ArgumentException("金额不能小于0!", nameof(input.TotalFee));

            if (string.IsNullOrWhiteSpace(input.Body)) throw new ArgumentNullException("商品描述必须填写!", nameof(input.Body));

            if (string.IsNullOrWhiteSpace(input.SpbillCreateIp))
                throw new ArgumentNullException("终端IP必须填写!", nameof(input.SpbillCreateIp));

            var url = "https://api.mch.weixin.qq.com/pay/unifiedorder";
            var config = GetConfig();

            var model = new NativeUnifiedorderRequest()
            {
                AppId = config.PayAppId,
                MchId = config.MchId,
                Attach = input.Attach,
                Body = input.Body,
                Detail = input.Detail,
                FeeType = input.FeeType,
                GoodsTag = input.GoodsTag,
                LimitPay = input.LimitPay,
                OutTradeNo = input.OutTradeNo ?? _weChatPayHelper.GenerateOutTradeNo(),
                SpbillCreateIp = input.SpbillCreateIp,
                TimeExpire = input.TimeExpire,
                TimeStart = input.TimeStart,
                TotalFee = ((int) (input.TotalFee * 100)).ToString(),
                NonceStr = _weChatPayHelper.GetNoncestr(),
                NotifyUrl = config.PayNotifyUrl
            };
            var dictionary = _weChatPayHelper.GetDictionaryByType(model);
            model.Sign = _weChatPayHelper.CreateMd5Sign(dictionary, config.TenPayKey); //生成Sign

            var result = _weChatPayHelper.PostXML<UnifiedorderResult>(url, model);
            if (result.IsSuccess())
            {
                var data = new
                {
                    appId = result.AppId,
                    nonceStr = result.NonceStr,
                    package = "prepay_id=" + result.PrepayId,
                    signType = "MD5",
                    timeStamp = _weChatPayHelper.GetTimestamp(),
                    codeUrl = result.CodeUrl
                };
                return new NativePayOutput
                {
                    AppId = data.appId,
                    Package = data.package,
                    NonceStr = data.nonceStr,
                    PaySign =
                        _weChatPayHelper.CreateMd5Sign(_weChatPayHelper.GetDictionaryByType(data), config.TenPayKey),
                    SignType = data.signType,
                    TimeStamp = data.timeStamp,
                    CodeUrl = data.codeUrl
                };
            }

            WeChatPayHelper.LoggerAction("Error", "支付错误：" + result.GetFriendlyMessage());
            throw new WeChatPayException("生成付款码出错，请联系客服或重新支付！");
        }

        #endregion

        #region 生成签名

        private string CreateSign<T>(T model)
        {
            var config = GetConfig();
            var dictionary = _weChatPayHelper.GetDictionaryByType(model);
            return _weChatPayHelper.CreateMd5Sign(dictionary, config.TenPayKey); //生成Sign
        }

        #endregion

        #region 支付回调处理

        /// <summary>
        ///     支付回调通知处理
        /// </summary>
        /// <param name="inputStream"></param>
        /// <returns></returns>
        public async Task<PayNotifyOutput> PayNotifyHandler(Stream inputStream)
        {
            PayNotifyOutput result = null;
            var data = await _weChatPayHelper.PostInput(inputStream);
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

        #endregion

        #region 处理支付回调参数

        /// <summary>
        ///     处理支付回调参数
        /// </summary>
        /// <param name="inputStream">输入流</param>
        /// <param name="payHandlerFunc">支付处理逻辑函数</param>
        /// <returns>处理结果</returns>
        public async Task<string> PayNotifyHandler(Stream inputStream, Action<PayNotifyOutput, string> payHandlerFunc)
        {
            PayNotifyOutput result = null;
            var data = await _weChatPayHelper.PostInput(inputStream);
            var outPutXml = string.Empty;
            var error = string.Empty;
            try
            {
                result = XmlHelper.DeserializeObject<PayNotifyOutput>(data);
            }
            catch (Exception ex)
            {
                WeChatPayHelper.LoggerAction?.Invoke("Error", "解析支付回调参数出错：" + data + "  Exception:" + ex);
                outPutXml = string.Format(FailXmlTpl, "解析支付回调参数出错");
                error = ex.ToString();
            }

            if (!string.IsNullOrWhiteSpace(outPutXml))
            {
            }
            else if (string.IsNullOrWhiteSpace(result?.TransactionId))
            {
                error = "支付结果中微信订单号不存在";
                outPutXml = string.Format(FailXmlTpl, error);
            }
            else if (!result.IsSuccess())
            {
                outPutXml = string.Format(FailXmlTpl, "回调处理失败");
                error = $"回调处理失败：ErrCode:{result.ErrCode} \nErrCodeDes:{result.ErrCodeDes}";
            }
            //查询订单，判断订单真实性
            else if (!QueryOrder(result.TransactionId))
            {
                error = "订单不存在";
                outPutXml = string.Format(FailXmlTpl, error);
            }

            payHandlerFunc?.Invoke(result, error);

            return !string.IsNullOrWhiteSpace(outPutXml)
                ? outPutXml
                : "<xml><return_code><![CDATA[SUCCESS]]></return_code></xml>";
        }

        #endregion

        #region 订单查询

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
                throw new WeChatPayException("订单查询接口中，out_trade_no、transaction_id至少填一个！");

            var config = GetConfig();
            var req = new OrderQueryRequest
            {
                AppId = config.PayAppId,
                MchId = config.MchId,
                NonceStr = _weChatPayHelper.GetNoncestr(),
                OutTradeNo = input.OutTradeNo,
                TransactionId = input.TransactionId,
                SignType = "MD5"
            };
            req.Sign = CreateSign(req);

            return _weChatPayHelper.PostXML<OrderQueryOutput>(url, req);
        }

        #endregion

        #region 企业付款

        /// <summary>
        ///     企业付款（提现）
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public EnterpriseResult EnterprisePayment(EnterpriseRequest model)
        {
            //发红包接口地址
            var url = "https://api.mch.weixin.qq.com/mmpaymkttransfers/promotion/transfers";
            if (string.IsNullOrEmpty(model.PartnerTradeNo))
                throw new ArgumentNullException("商户单号不能为空!", nameof(model.PartnerTradeNo));

            if (string.IsNullOrEmpty(model.Amount))
                throw new ArgumentNullException("必须输入企业付款金额!", nameof(model.PartnerTradeNo));
            if (string.IsNullOrEmpty(model.Desc))
                throw new ArgumentNullException("必须输入企业付款说明信息!", nameof(model.PartnerTradeNo));
            EnterpriseResult result = null;
            try
            {
                var config = GetConfig();
                model.MchAppId = config.PayAppId;
                model.MchId = config.MchId;
                //本地或者服务器的证书位置（证书在微信支付申请成功发来的通知邮件中）
                var cert = config.PayCertPath;

                //商户证书调用或安装都需要使用到密码，该密码的值为微信商户号（mch_id）
                var password = config.MchId;

                //调用证书
                var cer = new X509Certificate2(cert, password,
                    X509KeyStorageFlags.PersistKeySet | X509KeyStorageFlags.MachineKeySet);

                var dictionary = _weChatPayHelper.GetDictionaryByType(model);
                model.Sign = _weChatPayHelper.CreateMd5Sign(dictionary, config.TenPayKey); //生成Sign

                result = _weChatPayHelper.PostXML<EnterpriseResult>(url, model, cer);
            }
            catch (Exception ex)
            {
                WeChatPayHelper.LoggerAction?.Invoke(nameof(WeChatPayApi), ex.ToString());
            }

            return result;
        }

        #endregion

        #region 退款申请接口

        /// <summary>
        ///     退款申请接口
        /// </summary>
        /// <param name="model">The model<see cref="RefundRequest" /></param>
        /// <returns></returns>
        public RefundOutput Refund(RefundRequest model)
        {
            if (model == null) throw new ArgumentNullException(nameof(model));
            if (model.TotalFee < 0) throw new WeChatPayException("订单额不能小于0！");
            if (model.RefundFee < 0) throw new WeChatPayException("退款金额不能小于0！");
            if (string.IsNullOrEmpty(model.OutTradeNo) && string.IsNullOrEmpty(model.TransactionId))
                throw new WeChatPayException("商户订单号与微信订单号必须传入其中一个！");
            if (model.TotalFee < model.RefundFee) throw new WeChatPayException("退款金额不能大于订单金额！");
            var url = "https://api.mch.weixin.qq.com/secapi/pay/refund";

            RefundOutput result = null;
            try
            {
                var config = GetConfig();
                ;
                model.AppId = config.PayAppId;
                model.MchId = config.MchId;
                model.NonceStr = _weChatPayHelper.GetNoncestr();
                model.OpUserId = config.MchId;

                //本地或者服务器的证书位置（证书在微信支付申请成功发来的通知邮件中）
                var cert = config.PayCertPath;

                //商户证书调用或安装都需要使用到密码，该密码的值为微信商户号（mch_id）
                var password = config.MchId;

                //调用证书
                var cer = new X509Certificate2(cert, password,
                    X509KeyStorageFlags.PersistKeySet | X509KeyStorageFlags.MachineKeySet);

                var dictionary = _weChatPayHelper.GetDictionaryByType(model);

                model.Sign = _weChatPayHelper.CreateMd5Sign(dictionary, config.TenPayKey); //生成Sign
                result = _weChatPayHelper.PostXML<RefundOutput>(url, model, cer);
            }
            catch (Exception ex)
            {
                WeChatPayHelper.LoggerAction?.Invoke(nameof(WeChatPayApi), ex.ToString());
            }

            return result;
        }

        #endregion

        #region 普通红包发送

        /// <summary>
        ///     普通红包发送
        /// </summary>
        /// <param name="model">The model<see cref="NormalRedPackInput" /></param>
        /// <returns>The <see cref="NormalRedPackOutput" /></returns>
        public NormalRedPackOutput SendNormalRedPack(NormalRedPackInput model)
        {
            //发红包接口地址
            var url = "https://api.mch.weixin.qq.com/mmpaymkttransfers/sendredpack";

            NormalRedPackOutput result = null;
            var config = GetConfig();
            try
            {
                model.WxAppId = config.PayAppId;
                model.MchId = config.MchId;
                //本地或者服务器的证书位置（证书在微信支付申请成功发来的通知邮件中）
                var cert = config.PayCertPath;
                //私钥（在安装证书时设置）
                var password = config.CertPassword;

                //调用证书
                var cer = new X509Certificate2(cert, password,
                    X509KeyStorageFlags.PersistKeySet | X509KeyStorageFlags.MachineKeySet);
                //ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CheckValidationResult);
                //X509Certificate cer = new X509Certificate(cert, password);

                var dictionary = _weChatPayHelper.GetDictionaryByType(model);
                model.Sign = _weChatPayHelper.CreateMd5Sign(dictionary, config.TenPayKey); //生成Sign
                //var dict = PayUtil.GetAuthors(model);

                result = _weChatPayHelper.PostXML<NormalRedPackOutput>(url, model, cer);
            }
            catch (Exception ex)
            {
                WeChatPayHelper.LoggerAction?.Invoke(nameof(WeChatPayApi), ex.ToString());
            }

            return result;
        }

        #endregion

        #region 查询订单是否存在

        /// <summary>
        ///     查询订单是否存在
        /// </summary>
        /// <param name="transactionId"></param>
        /// <returns></returns>
        private bool QueryOrder(string transactionId)
        {
            return OrderQuery(new OrderQueryInput
            {
                TransactionId = transactionId
            }).IsSuccess();
        }

        public string GetOpenId(string code)
        {
            var config = GetConfig();
            string url =
                $"https://api.weixin.qq.com/sns/oauth2/access_token?appid={config.PayAppId}&secret={config.AppSecret}&code={code}&grant_type=authorization_code";
            var httpResult = RequestUtility.HttpGet(url, Encoding.UTF8);
            WeChatPayHelper.LoggerAction("Debug", "请求结果：" + httpResult);
            var token =JsonConvert.DeserializeObject<AccessToken>(httpResult);
            return token.openid;
        }
        #endregion
    }
}
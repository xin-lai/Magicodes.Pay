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
        internal static Func<IWeChatPayConfig> GetPayConfigFunc { get; set; } = () => null;
        internal static Action<string, string> LoggerAction = (tag, log) => { };

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

            var config = GetPayConfigFunc();
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
                OutTradeNo = input.OutTradeNo ?? GenerateOutTradeNo(),
                SpbillCreateIp = input.SpbillCreateIp,
                TimeExpire = input.TimeExpire,
                TimeStart = input.TimeStart,
                TotalFee = ((int)(input.TotalFee * 100)).ToString(),
            };
            model.NonceStr = GetNoncestr();
            model.NotifyUrl = config.PayNotifyUrl;
            var dictionary = GetAuthors(model);
            model.Sign = CreateMd5Sign(dictionary, config.TenPayKey); //生成Sign

            var result = PostXML<UnifiedorderResult>(url, model);
            if (result.IsSuccess())
            {
                var data = new
                {
                    appId = result.AppId,
                    nonceStr = result.NonceStr,
                    package = "prepay_id=" + result.PrepayId,
                    signType = "MD5",
                    timeStamp = GetTimestamp(),
                };
                return new MiniProgramPayOutput()
                {
                    AppId = data.appId,
                    Package = data.package,
                    NonceStr = data.nonceStr,
                    PaySign = CreateMd5Sign(GetAuthors(data), config.TenPayKey),
                    SignType = data.signType,
                    TimeStamp = data.timeStamp
                };
            }
            LoggerAction("Error", "支付错误：" + result.GetFriendlyMessage());
            throw new PayException("支付错误，请联系客服或重新支付！");
        }

        /// <summary>
        /// 支付回调通知处理
        /// </summary>
        /// <param name="inputStream"></param>
        /// <returns></returns>
        public PayNotifyOutput PayNotifyHandler(Stream inputStream)
        {
            PayNotifyOutput result = null;
            var data = PostInput(inputStream);
            try
            {
                result = XmlHelper.DeserializeObject<PayNotifyOutput>(data);
            }
            catch (Exception ex)
            {
                LoggerAction?.Invoke("Error", "解析支付回调参数出错：" + data + "  Exception:" + ex.ToString());
                throw;
            }
            return result;
        }

        #region 私有方法
        /// <summary>
        ///     随机生成Noncestr
        /// </summary>
        /// <returns></returns>
        private string GetNoncestr()
        {
            return Guid.NewGuid().ToString("N");
            //var random = new Random();
            //return MD5UtilHelper.GetMD5(random.Next(1000).ToString(), "GBK");
        }

        /// <summary>
        ///     根据当前系统时间加随机序列来生成订单号
        /// </summary>
        /// <returns>订单号</returns>
        private string GenerateOutTradeNo()
        {
            var ran = new Random();
            return string.Format("{0}{1}", UnixStamp(), ran.Next(999));
        }

        /// <summary>
        ///     获取时间戳
        /// </summary>
        /// <returns></returns>
        private string GetTimestamp()
        {
            var ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return Convert.ToInt64(ts.TotalSeconds).ToString();
        }

        /// <summary>
        ///     对字符串进行URL编码
        /// </summary>
        /// <param name="instr"></param>
        /// <param name="charset"></param>
        /// <returns></returns>
        private string UrlEncode(string instr, string charset)
        {
            //return instr;
            if (instr == null || instr.Trim() == "")
                return "";
            return instr.UrlEncode();
        }

        /// <summary>
        ///     对字符串进行URL解码
        /// </summary>
        /// <param name="instr"></param>
        /// <param name="charset"></param>
        /// <returns></returns>
        private string UrlDecode(string instr, string charset)
        {
            if (instr == null || instr.Trim() == "")
                return "";
            var res = instr.UrlDecode();
            //try
            //{
            //    res = HttpUtility.UrlDecode(instr, Encoding.GetEncoding(charset));
            //}
            //catch (Exception ex)
            //{
            //    res = HttpUtility.UrlDecode(instr, Encoding.GetEncoding("GB2312"));
            //}

            return res;
        }


        /// <summary>
        ///     取时间戳生成随即数,替换交易单号中的后10位流水号
        /// </summary>
        /// <returns></returns>
        private uint UnixStamp()
        {
            var ts = DateTime.Now - TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            return Convert.ToUInt32(ts.TotalSeconds);
        }

        /// <summary>
        ///     取随机数
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        private string BuildRandomStr(int length)
        {
            var rand = new Random();

            var num = rand.Next();

            var str = num.ToString();

            if (str.Length > length)
            {
                str = str.Substring(0, length);
            }
            else if (str.Length < length)
            {
                var n = length - str.Length;
                while (n > 0)
                {
                    str.Insert(0, "0");
                    n--;
                }
            }

            return str;
        }

        /// <summary>
        ///     循环获取一个实体类每个字段的XmlAttribute属性的值
        /// </summary>
        /// <typeparam name="T">
        ///     <peparam>
        ///         <returns></returns>
        private Dictionary<string, string> GetAuthors<T>(T model)
        {
            var _dict = new Dictionary<string, string>();

            var type = model.GetType(); //获取类型

            var props = typeof(T).GetProperties();
            foreach (var prop in props)
            {
                var attrs = prop.GetCustomAttributes(true);
                XmlElementAttribute attr = null;
                if (attrs.Length > 0)
                {
                    attr = attrs[0] as XmlElementAttribute;
                }
                var property = type.GetProperty(prop.Name);
                var value = property.GetValue(model, null); //获取属性值
                _dict.Add(attr?.ElementName ?? property.Name, value?.ToString());

            }
            return _dict;
        }

        /// <summary>
        ///     创建md5摘要,规则是:按参数名称a-z排序,遇到空值的参数不参加签名
        /// </summary>
        /// <param name="value">参数值</param>
        /// <returns></returns>
        private string CreateMd5Sign(Dictionary<string, string> dict, string value)
        {
            var akeys = new ArrayList();
            foreach (var x in dict)
            {
                if ("sign".CompareTo(x.Key) == 0)
                    continue;
                akeys.Add(x.Key);
            }
            var sb = new StringBuilder();
            akeys.Sort();

            foreach (string k in akeys)
            {
                var v = dict[k];
                if (null != v && "".CompareTo(v) != 0
                    && "sign".CompareTo(k) != 0 && "key".CompareTo(k) != 0)
                    sb.Append(k + "=" + v + "&");
            }
            sb.Append("key=" + value);
            var md5 = MD5.Create();
            var bs = md5.ComputeHash(Encoding.UTF8.GetBytes(sb.ToString()));
            var sbuilder = new StringBuilder();
            foreach (var b in bs)
                sbuilder.Append(b.ToString("x2"));
            //所有字符转为大写
            return sbuilder.ToString().ToUpper();
        }

        /// <summary>
        ///     接收post数据
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private string PostInput(Stream stream)
        {
            var count = 0;
            var buffer = new byte[1024];
            var builder = new StringBuilder();
            while ((count = stream.Read(buffer, 0, 1024)) > 0)
                builder.Append(Encoding.UTF8.GetString(buffer, 0, count));
            return builder.ToString();
        }

        /// <summary>
        ///     POST提交请求，返回ApiResult对象
        /// </summary>
        /// <typeparam name="T">ApiResult对象</typeparam>
        /// <param name="url">请求地址</param>
        /// <param name="obj">提交的数据对象</param>
        /// <returns>ApiResult对象</returns>
        protected T PostXML<T>(string url, object obj, Func<string, string> serializeStrFunc = null) where T : PayOutputBase
        {
            var wr = new WeChatApiWebRequestHelper();
            string resultStr = null;
            var result = wr.HttpPost<T>(url, obj, out resultStr, serializeStrFunc,
                WebRequestDataTypes.XML, WebRequestDataTypes.XML);
            if (result != null)
                result.DetailResult = resultStr;
            return result;
        }

        /// <summary>
        ///     POST提交请求，带证书，返回ApiResult对象
        /// </summary>
        /// <typeparam name="T">ApiResult对象</typeparam>
        /// <param name="url">请求地址</param>
        /// <param name="obj">提交的数据对象</param>
        /// <returns>ApiResult对象</returns>
        protected T PostXML<T>(string url, object obj, X509Certificate2 cer,
            Func<string, string> serializeStrFunc = null) where T : PayOutputBase
        {
            var wr = new WeChatApiWebRequestHelper();
            string resultStr = null;
            var result = wr.HttpPost<T>(url, obj, cer, out resultStr, serializeStrFunc,
                WebRequestDataTypes.XML, WebRequestDataTypes.XML);
            if (result != null)
                result.DetailResult = resultStr;
            return result;
        }
        #endregion
    }
}

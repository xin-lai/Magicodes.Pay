// ======================================================================
//   
//           Copyright (C) 2018-2020 湖南心莱信息科技有限公司    
//           All rights reserved
//   
//           filename : WeChatPayHelper.cs
//           description :
//   
//           created by 雪雁 at  2018-07-30 20:29
//           Mail: wenqiang.li@xin-lai.com
//           QQ群：85318032（技术交流）
//           Blog：http://www.cnblogs.com/codelove/
//           GitHub：https://github.com/xin-lai
//           Home：http://xin-lai.com
//   
// ======================================================================

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Xml.Serialization;
using Magicodes.Pay.Wxpay.Config;
using static System.TimeZone;

namespace Magicodes.Pay.Wxpay.Helper
{
    public class WeChatPayHelper
    {
        internal static Action<string, string> LoggerAction = (tag, log) => { };
        internal static Func<IWeChatPayConfig> GetPayConfigFunc { get; set; } = () => null;

        #region 方法

        /// <summary>
        ///     随机生成Noncestr
        /// </summary>
        /// <returns></returns>
        internal string GetNoncestr()
        {
            return Guid.NewGuid().ToString("N");
        }

        /// <summary>
        ///     根据当前系统时间加随机序列来生成订单号
        /// </summary>
        /// <returns>订单号</returns>
        internal string GenerateOutTradeNo()
        {
            var ran = new Random();
            return $"{UnixStamp()}{ran.Next(999)}";
        }

        /// <summary>
        ///     获取时间戳
        /// </summary>
        /// <returns></returns>
        internal string GetTimestamp()
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
        internal string UrlEncode(string instr, string charset)
        {
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
        internal string UrlDecode(string instr, string charset)
        {
            if (instr == null || instr.Trim() == "")
                return "";
            var res = instr.UrlDecode();
            return res;
        }


        /// <summary>
        ///     取时间戳生成随即数,替换交易单号中的后10位流水号
        /// </summary>
        /// <returns></returns>
        internal uint UnixStamp()
        {
            var ts = DateTime.Now - CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            return Convert.ToUInt32(ts.TotalSeconds);
        }

        /// <summary>
        ///     取随机数
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        internal string BuildRandomStr(int length)
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
        internal Dictionary<string, string> GetDictionaryByType<T>(T model)
        {
            var dict = new Dictionary<string, string>();

            var type = model.GetType(); //获取类型

            var props = typeof(T).GetProperties();
            foreach (var prop in props)
            {
                var attrs = prop.GetCustomAttributes(true);
                XmlElementAttribute attr = null;
                if (attrs.Length > 0) attr = attrs[0] as XmlElementAttribute;
                var property = type.GetProperty(prop.Name);
                var value = property.GetValue(model, null); //获取属性值
                dict.Add(attr?.ElementName ?? property.Name, value?.ToString());
            }

            return dict;
        }

        /// <summary>
        ///     创建md5摘要,规则是:按参数名称a-z排序,遇到空值的参数不参加签名
        /// </summary>
        /// <param name="value">参数值</param>
        /// <returns></returns>
        internal string CreateMd5Sign(Dictionary<string, string> dict, string value)
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
        /// <returns></returns>
        internal string PostInput(Stream stream)
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
        internal T PostXML<T>(string url, object obj, Func<string, string> serializeStrFunc = null)
            where T : PayOutputBase
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
        internal T PostXML<T>(string url, object obj, X509Certificate2 cer,
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
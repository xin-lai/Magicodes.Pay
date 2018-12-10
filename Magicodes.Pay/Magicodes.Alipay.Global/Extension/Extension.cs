// ======================================================================
//   
//           Copyright (C) 2018-2020 湖南心莱信息科技有限公司    
//           All rights reserved
//   
//           filename : Extension.cs
//           description :
//   
//           created by 雪雁 at  2018-11-23 11:00
//           Mail: wenqiang.li@xin-lai.com
//           QQ群：85318032（技术交流）
//           Blog：http://www.cnblogs.com/codelove/
//           GitHub：https://github.com/xin-lai
//           Home：http://xin-lai.com
//   
// ======================================================================

using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace Magicodes.Alipay.Global.Extension
{
    public static class Extension
    {
        /// <summary>
        ///     除去数组中的空值和签名参数并以字母a到z的顺序排序
        /// </summary>
        /// <param name="dicArrayPre">过滤前的参数组</param>
        /// <returns>过滤后的参数组</returns>
        public static Dictionary<string, string> FilterPara(this SortedDictionary<string, string> dicArrayPre)
        {
            var dicArray = new Dictionary<string, string>();
            foreach (var temp in dicArrayPre)
                if (temp.Key.ToLower() != "sign" && temp.Key.ToLower() != "sign_type" &&
                    !string.IsNullOrEmpty(temp.Value))
                    dicArray.Add(temp.Key, temp.Value);

            return dicArray;
        }

        /// <summary>
        ///     生成请求时的签名
        /// </summary>
        /// <param name="sPara">请求给支付宝的参数数组</param>
        /// <param name="alipaySettings"></param>
        /// <returns>签名结果</returns>
        private static string BuildRequestMysign(this Dictionary<string, string> sPara,
            IGlobalAlipaySettings alipaySettings)
        {
            //把数组所有元素，按照“参数=参数值”的模式用“&”字符拼接成字符串
            var prestr = sPara.CreateLinkString();

            //把最终的字符串签名，获得签名结果
            var mysign = "";
            switch (alipaySettings.SignType)
            {
                case "MD5":
                    mysign = Sign(prestr, alipaySettings.Key, alipaySettings.CharSet);
                    break;
                default:
                    mysign = "";
                    break;
            }

            return mysign;
        }

        /// <summary>
        ///     签名字符串
        /// </summary>
        /// <param name="prestr">需要签名的字符串</param>
        /// <param name="key">密钥</param>
        /// <param name="inputCharset">编码格式</param>
        /// <returns>签名结果</returns>
        public static string Sign(string prestr, string key, string inputCharset)
        {
            var sb = new StringBuilder(32);

            prestr = prestr + key;

            MD5 md5 = new MD5CryptoServiceProvider();
            var t = md5.ComputeHash(Encoding.GetEncoding(inputCharset).GetBytes(prestr));
            foreach (var t1 in t) sb.Append(t1.ToString("x").PadLeft(2, '0'));

            return sb.ToString();
        }

        /// <summary>
        ///     验证签名
        /// </summary>
        /// <param name="prestr">需要签名的字符串</param>
        /// <param name="sign">签名结果</param>
        /// <param name="key">密钥</param>
        /// <param name="inputCharset">编码格式</param>
        /// <returns>验证结果</returns>
        public static bool Verify(string prestr, string sign, string key, string inputCharset)
        {
            var mysign = Sign(prestr, key, inputCharset);
            return mysign == sign;
        }

        /// <summary>
        ///     把数组所有元素，按照“参数=参数值”的模式用“&”字符拼接成字符串
        /// </summary>
        /// <param name="dicArray">需要拼接的数组</param>
        /// <returns>拼接完成以后的字符串</returns>
        public static string CreateLinkString(this Dictionary<string, string> dicArray)
        {
            var prestr = new StringBuilder();
            foreach (var temp in dicArray) prestr.Append(temp.Key + "=" + temp.Value + "&");

            //去掉最後一個&字符
            var nLen = prestr.Length;
            prestr.Remove(nLen - 1, 1);

            return prestr.ToString();
        }

        /// <summary>
        ///     把数组所有元素，按照“参数=参数值”的模式用“&”字符拼接成字符串，并对参数值做urlencode
        /// </summary>
        /// <param name="dicArray">需要拼接的数组</param>
        /// <returns>拼接完成以后的字符串</returns>
        public static string CreateLinkStringUrlencode(this Dictionary<string, string> dicArray)
        {
            var prestr = new StringBuilder();
            foreach (var temp in dicArray)
                prestr.Append(temp.Key + "=" + HttpUtility.UrlEncode(temp.Value, Encoding.UTF8) + "&");

            //去掉最後一個&字符
            var nLen = prestr.Length;
            prestr.Remove(nLen - 1, 1);

            return prestr.ToString();
        }

        /// <summary>
        ///     获取文件的md5摘要
        /// </summary>
        /// <param name="sFile">文件流</param>
        /// <returns>MD5摘要结果</returns>
        public static string GetAbstractToMD5(this Stream sFile)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            var result = md5.ComputeHash(sFile);
            var sb = new StringBuilder(32);
            foreach (var t in result) sb.Append(t.ToString("x").PadLeft(2, '0'));
            return sb.ToString();
        }

        /// <summary>
        ///     获取文件的md5摘要
        /// </summary>
        /// <param name="dataFile">文件流</param>
        /// <returns>MD5摘要结果</returns>
        public static string GetAbstractToMD5(this byte[] dataFile)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            var result = md5.ComputeHash(dataFile);
            var sb = new StringBuilder(32);
            foreach (var t in result) sb.Append(t.ToString("x").PadLeft(2, '0'));
            return sb.ToString();
        }

        /// <summary>
        ///     生成要请求给支付宝的参数数组
        /// </summary>
        /// <param name="sParaTemp">请求前的参数数组</param>
        /// <param name="alipaySettings"></param>
        /// <returns>要请求的参数数组</returns>
        public static Dictionary<string, string> BuildRequestPara(this SortedDictionary<string, string> sParaTemp,
            IGlobalAlipaySettings alipaySettings)
        {
            //待签名请求参数数组
            //签名结果
            var mysign = "";

            //过滤签名参数数组
            var sPara = FilterPara(sParaTemp);

            //获得签名结果
            mysign = BuildRequestMysign(sPara, alipaySettings);

            //签名结果与签名方式加入请求提交参数组中
            sPara.Add("sign", mysign);
            sPara.Add("sign_type", alipaySettings.SignType);

            return sPara;
        }

        /// <summary>
        ///     生成要请求给支付宝的参数数组
        /// </summary>
        /// <param name="sParaTemp">请求前的参数数组</param>
        /// <param name="alipaySettings"></param>
        /// <returns>要请求的参数数组字符串</returns>
        public static string BuildRequestParaToString(this SortedDictionary<string, string> sParaTemp,
            IGlobalAlipaySettings alipaySettings)
        {
            //待签名请求参数数组
            var sPara = BuildRequestPara(sParaTemp, alipaySettings);

            //把参数组中所有元素，按照“参数=参数值”的模式用“&”字符拼接成字符串，并对参数值做urlencode
            var strRequestData = CreateLinkStringUrlencode(sPara);

            return strRequestData;
        }

        public static string GetHtmlSubmitForm(this Dictionary<string, string> dicPara, string gatewayurl,
            string charSet)
        {
            var sbHtml = new StringBuilder();

            sbHtml.Append("<form id='alipaysubmit' name='alipaysubmit' action='" + gatewayurl + "' _input_charset=" +
                          charSet + "' method='get'>");

            foreach (var temp in dicPara)
                sbHtml.Append("<input type='hidden' name='" + temp.Key + "' value='" + temp.Value + "'/>");

            //submit按钮控件请不要含有name属性
            sbHtml.Append("<input type='submit' value='支付宝' style='display:none;'></form>");

            sbHtml.Append("<script>document.forms['alipaysubmit'].submit();</script>");

            return sbHtml.ToString();
        }


        ///// <summary>
        ///// 用于防钓鱼，调用接口query_timestamp来获取时间戳的处理函数
        ///// 注意：远程解析XML出错，与IIS服务器配置有关
        ///// </summary>
        ///// <returns>时间戳字符串</returns>
        //public static string Query_timestamp()
        //{
        //    string url = GATEWAY_NEW + "service=query_timestamp&partner=" + Config.partner + "&_input_charset=" + Config.input_charset;
        //    var encrypt_key = "";

        //    var Reader = new XmlTextReader(url);
        //    var xmlDoc = new XmlDocument();
        //    xmlDoc.Load(Reader);

        //    encrypt_key = xmlDoc.SelectSingleNode("/alipay/response/timestamp/encrypt_key").InnerText;

        //    return encrypt_key;
        //}
    }
}
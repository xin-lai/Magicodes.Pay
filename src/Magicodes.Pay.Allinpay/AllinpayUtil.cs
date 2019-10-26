using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;

namespace Magicodes.Pay.Allinpay
{
    public class AllinpayUtil
    {
        /// <summary>
        /// 将参数排序组装
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static string BuildParamStr(Dictionary<string, string> param)
        {
            if (param == null || param.Count == 0)
            {
                return "";
            }
            var ascDic = param.OrderBy(o => o.Key).ToDictionary(o => o.Key, p => p.Value);
            var sb = new StringBuilder();
            foreach (var item in ascDic)
            {
                if (!string.IsNullOrEmpty(item.Value))
                {
                    sb.Append(item.Key).Append("=").Append(item.Value).Append("&");
                }
            }
            return sb.ToString().Substring(0,sb.ToString().Length-1);
        }

        public static string SignParam(Dictionary<string, string> param, string appKey)
        {
            if (param == null || param.Count == 0)
            {
                return "";
            }
            param.Add("key", appKey);
            var blankStr = BuildParamStr(param);
            return Md5Encrypt(blankStr);

        }

        public static bool ValidSign(Dictionary<string, string> param, string appKey)
        {
            var signRsp = param["sign"];
            param.Remove("sign");
            var sign = SignParam(param, appKey);
            return sign.ToLower().Equals(signRsp.ToLower());

        }

        /// <summary>
        /// 将实体转化为json
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public static string ObjectToJson(object o)
        {
            var json = JsonConvert.SerializeObject(o);
            return json;
        }

        /// <summary>
        /// md5加签
        /// </summary>
        /// <param name="strText"></param>
        /// <returns></returns>
        public static string Md5Encrypt(string strText)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            var result = md5.ComputeHash(System.Text.Encoding.UTF8.GetBytes(strText));
            return BitConverter.ToString(result).Replace("-", "");
        }  
    }
}
// ======================================================================
//   
//           Copyright (C) 2018-2020 湖南心莱信息科技有限公司    
//           All rights reserved
//   
//           filename : NotifyHelper.cs
//           description :
//   
//           created by 雪雁 at  2018-11-23 14:54
//           Mail: wenqiang.li@xin-lai.com
//           QQ群：85318032（技术交流）
//           Blog：http://www.cnblogs.com/codelove/
//           GitHub：https://github.com/xin-lai
//           Home：http://xin-lai.com
//   
// ======================================================================

using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using Magicodes.Alipay.Global.Extension;

namespace Magicodes.Alipay.Global.Helper
{
    public class NotifyHelper
    {
        //支付宝消息验证地址
        private const string Https_veryfy_url = "https://mapi.alipay.com/gateway.do?service=notify_verify&";

        /// <summary>
        ///     从文件读取公钥转公钥字符串
        /// </summary>
        /// <param name="path">公钥文件路径</param>
        public static string GetPublicKeyStr(string path)
        {
            string pubkey;
            using (var sr = new StreamReader(path))
            {
                pubkey = sr.ReadToEnd();
                sr.Close();
            }

            pubkey = pubkey.Replace("-----BEGIN PUBLIC KEY-----", "");
            pubkey = pubkey.Replace("-----END PUBLIC KEY-----", "");
            pubkey = pubkey.Replace("\r", "");
            pubkey = pubkey.Replace("\n", "");
            return pubkey;
        }


        /// <summary>
        ///     验证消息是否是支付宝发出的合法消息
        /// </summary>
        /// <param name="inputPara">通知返回参数数组</param>
        /// <param name="notifyId">通知验证ID</param>
        /// <param name="sign">支付宝生成的签名结果</param>
        /// <param name="alipaySettings"></param>
        /// <returns>验证结果</returns>
        public bool Verify(SortedDictionary<string, string> inputPara, string notifyId, string sign,
            IGlobalAlipaySettings alipaySettings)
        {
            //获取返回时的签名验证结果
            var isSign = GetSignVeryfy(inputPara, sign, alipaySettings);
            //获取是否是支付宝服务器发来的请求的验证结果
            var responseTxt = "false";
            if (!string.IsNullOrEmpty(notifyId)) responseTxt = GetResponseTxt(notifyId, alipaySettings);

            //写日志记录（若要调试，请取消下面两行注释）
            var sWord = "responseTxt=" + responseTxt + "\n isSign=" + isSign + "\n 返回的参数：" + GetPreSignStr(inputPara) +
                        "\n ";

            GlobalAlipayAppService.LoggerAction("Debug", sWord);

            //判断responsetTxt是否为true，isSign是否为true
            //responsetTxt的结果不是true，与服务器设置问题、合作身份者ID、notify_id一分钟失效有关
            //isSign不是true，与安全校验码、请求时的参数格式（如：带自定义参数等）、编码格式有关
            return responseTxt == "true" && isSign;
        }

        /// <summary>
        ///     验证消息是否是支付宝发出的消息
        /// </summary>
        /// <param name="inputPara">通知返回参数数组</param>
        /// <param name="sign">支付宝生成的签名结果</param>
        /// <returns>验证结果</returns>
        public bool VerifyReturn(SortedDictionary<string, string> inputPara, string sign,
            IGlobalAlipaySettings alipaySettings)
        {
            //获取返回时的签名验证结果
            var isSign = GetSignVeryfy(inputPara, sign, alipaySettings);
            return isSign;
        }

        /// <summary>
        ///     获取待签名字符串（调试用）
        /// </summary>
        /// <param name="inputPara">通知返回参数数组</param>
        /// <returns>待签名字符串</returns>
        private string GetPreSignStr(SortedDictionary<string, string> inputPara)
        {
            //过滤空值、sign与sign_type参数
            var sPara = inputPara.FilterPara();

            //获取待签名字符串
            var preSignStr = sPara.CreateLinkString();

            return preSignStr;
        }

        /// <summary>
        ///     获取返回时的签名验证结果
        /// </summary>
        /// <param name="inputPara">通知返回参数数组</param>
        /// <param name="sign">对比的签名结果</param>
        /// <returns>签名验证结果</returns>
        private bool GetSignVeryfy(SortedDictionary<string, string> inputPara, string sign,
            IGlobalAlipaySettings alipaySettings)
        {
            //过滤空值、sign与sign_type参数
            var sPara = inputPara.FilterPara();

            //获取待签名字符串
            var preSignStr = sPara.CreateLinkString();

            //获得签名验证结果
            var isSgin = false;
            if (string.IsNullOrEmpty(sign)) return false;
            switch (alipaySettings.SignType)
            {
                case "MD5":
                    isSgin = Extension.Extension.Verify(preSignStr, sign, alipaySettings.Key, alipaySettings.CharSet);
                    break;
                default:
                    break;
            }

            return isSgin;
        }

        /// <summary>
        ///     获取是否是支付宝服务器发来的请求的验证结果
        /// </summary>
        /// <param name="notifyId">通知验证ID</param>
        /// <returns>验证结果</returns>
        private string GetResponseTxt(string notifyId, IGlobalAlipaySettings alipaySettings)
        {
            var veryfyUrl = $"{Https_veryfy_url}partner={alipaySettings.Partner}&notify_id={notifyId}";

            //获取远程服务器ATN结果，验证是否是支付宝服务器发来的请求
            var responseTxt = Get_Http(veryfyUrl, 120000);

            return responseTxt;
        }

        /// <summary>
        ///     获取远程服务器ATN结果
        /// </summary>
        /// <param name="strUrl">指定URL路径地址</param>
        /// <param name="timeout">超时时间设置</param>
        /// <returns>服务器ATN结果</returns>
        private string Get_Http(string strUrl, int timeout)
        {
            string strResult;
            try
            {
                var myReq = WebRequest.Create(strUrl);
                myReq.Timeout = timeout;
                var res = myReq.GetResponse();
                using (var myStream = res.GetResponseStream())
                {
                    var sr = new StreamReader(myStream ?? throw new InvalidOperationException("获取远程结果为空"),
                        Encoding.Default);
                    var strBuilder = new StringBuilder();
                    while (-1 != sr.Peek()) strBuilder.Append(sr.ReadLine());

                    strResult = strBuilder.ToString();
                }
            }
            catch (Exception exp)
            {
                strResult = "错误：" + exp.Message;
            }

            return strResult;
        }
    }
}
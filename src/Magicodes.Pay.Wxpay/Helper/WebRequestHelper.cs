// ======================================================================
//   
//           Copyright (C) 2018-2020 湖南心莱信息科技有限公司    
//           All rights reserved
//   
//           filename : WebRequestHelper.cs
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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace Magicodes.Pay.Wxpay.Helper
{
    /// <summary>
    ///     ** 描述：模拟HTTP POST GET请求并获取数据
    ///     使用：WebRequestHelper wr = new WebRequestHelper();
    ///     //GET
    ///     var html= ws.HttpGet("http://WeiChat.Magicodes.net");
    ///     //带参GET
    ///     var paras=new Dictionary
    ///     <string, string>
    ///         () ;
    ///         paras.Add("name","skx");
    ///         paras.Add("id", "100");
    ///         var html2 = ws.HttpGet("http://WeiChat.Magicodes.net",paras );
    ///         //POST
    ///         var postHtml= ws.HttpPost("http://WeiChat.Magicodes.net", paras);
    ///         //post and file
    ///         var postHtml2 = ws.HttpUploadFile("http://WeiChat.Magicodes.net", "文件地址可以是数组", paras);
    /// </summary>
    public class WebRequestHelper
    {
        /// <summary>
        ///     accept
        /// </summary>
        private string accept = "*/*";

        /// <summary>
        ///     是否允许重定向
        /// </summary>
        private bool allowAutoRedirect = true;

        /// <summary>
        ///     contentType
        /// </summary>
        private string contentType = "application/x-www-form-urlencoded";

        /// <summary>
        ///     设置cookie
        /// </summary>
        private CookieContainer cookie;

        /// <summary>
        ///     过期时间（默认：30000）
        /// </summary>
        private int timeOut = 30000;

        /// <summary>
        ///     代理密码
        /// </summary>
        public string ProxyKey { get; set; }

        /// <summary>
        ///     代理地址
        /// </summary>
        public string ProxyAddress { get; set; }


        /// <summary>
        ///     代理用户
        /// </summary>
        public string ProxyUser { get; set; }

        public CookieContainer Cookie
        {
            get => cookie;
            set => cookie = value;
        }

        /// <summary>
        ///     是否允许重定向(默认:true)
        /// </summary>
        public bool AllowAutoRedirect
        {
            get => allowAutoRedirect;
            set => allowAutoRedirect = value;
        }

        /// <summary>
        ///     设置contentType(默认:application/x-www-form-urlencoded)
        /// </summary>
        public string ContentType
        {
            get => contentType;
            set => contentType = value;
        }

        /// <summary>
        ///     设置accept(默认:*/*)
        /// </summary>
        public string Accept
        {
            get => accept;
            set => accept = value;
        }

        public int TimeOut
        {
            get => timeOut;
            set => timeOut = value;
        }

        /// <summary>
        /// </summary>
        public string UserAgent { get; set; } =
            "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/31.0.1650.57 Safari/537.36";

        /// <summary>
        ///     接受的语言
        /// </summary>
        public string AcceptLanguage { get; set; }

        /// <summary>
        ///     处理POST请求
        /// </summary>
        /// <param name="url"></param>
        /// <param name="postdata"></param>
        /// <returns></returns>
        public string HttpPost(string url, string postdata)
        {
            var request = CreateWebRequest(url);
            request.Method = "POST";
            if (!string.IsNullOrWhiteSpace(postdata))
            {
                var bytesToPost = Encoding.UTF8.GetBytes(postdata);
                request.ContentLength = bytesToPost.Length;
                using (var requestStream = request.GetRequestStream())
                {
                    requestStream.Write(bytesToPost, 0, bytesToPost.Length);
                    requestStream.Close();
                }
            }

            using (var response = (HttpWebResponse) request.GetResponse())
            {
                using (var sr = new StreamReader(response.GetResponseStream()))
                {
                    var result = sr.ReadToEnd();
                    return result;
                }
            }
        }

        /// <summary>
        ///     带证书的post请求
        /// </summary>
        /// <param name="url"></param>
        /// <param name="postdata"></param>
        /// <param name="cer"></param>
        /// <returns></returns>
        public string HttpPost(string url, string postdata, X509Certificate cer)
        {
            var request = CreateWebRequest(url);
            request.ClientCertificates.Add(cer);
            request.Method = "POST";
            if (!string.IsNullOrWhiteSpace(postdata))
            {
                var bytesToPost = Encoding.UTF8.GetBytes(postdata);
                request.ContentLength = bytesToPost.Length;
                using (var requestStream = request.GetRequestStream())
                {
                    requestStream.Write(bytesToPost, 0, bytesToPost.Length);
                    requestStream.Close();
                }
            }

            using (var response = (HttpWebResponse) request.GetResponse())
            {
                using (var sr = new StreamReader(response.GetResponseStream()))
                {
                    var result = sr.ReadToEnd();
                    return result;
                }
            }
        }

        public T HttpPost<T>(string url, object obj, out string result, Func<string, string> serializeStrFunc = null,
            WebRequestDataTypes inputDataType = WebRequestDataTypes.JSON,
            WebRequestDataTypes outDataType = WebRequestDataTypes.JSON) where T : class
        {
            string postStr = null;
            switch (inputDataType)
            {
                case WebRequestDataTypes.XML:
                    postStr = XmlHelper.SerializeObject(obj);
                    break;
                default:
                    postStr = JsonConvert.SerializeObject(obj);
                    break;
            }

            if (serializeStrFunc != null)
                postStr = serializeStrFunc(postStr);
            WeChatPayHelper.LoggerAction?.Invoke("api", string.Format("Pre POST Url:{0},Data:{1}", url, postStr));
            result = HttpPost(url, postStr);
            WeChatPayHelper.LoggerAction?.Invoke("api", string.Format("POST Url:{0},result:{1}", url, result));
            switch (outDataType)
            {
                case WebRequestDataTypes.XML:
                    return XmlHelper.DeserializeObject<T>(result);
                default:
                    return JsonConvert.DeserializeObject<T>(result);
            }
        }

        public T HttpPost<T>(string url, string fileName, Stream fileStream, out string result,
            WebRequestDataTypes outDataType = WebRequestDataTypes.JSON) where T : class
        {
            var boundary = "---------------------------" + DateTime.Now.Ticks.ToString("x");
            var boundarybytes = Encoding.UTF8.GetBytes("\r\n--" + boundary + "\r\n");
            var endbytes = Encoding.UTF8.GetBytes("\r\n--" + boundary + "--\r\n");
            var request = CreateWebRequest(url);
            request.ContentType = "multipart/form-data; boundary=" + boundary;
            request.KeepAlive = true;
            request.AllowAutoRedirect = AllowAutoRedirect;
            request.Method = "POST";
            if (Cookie != null)
                request.CookieContainer = Cookie;
            request.Credentials = CredentialCache.DefaultCredentials;

            using (var requestStream = request.GetRequestStream())
            {
                var headerTemplate =
                    "Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"\r\nContent-Type: application/octet-stream\r\n\r\n";
                var buffer = new byte[4096];
                var bytesRead = 0;
                requestStream.Write(boundarybytes, 0, boundarybytes.Length);
                var header = string.Format(headerTemplate, fileName, fileName);
                var headerbytes = Encoding.UTF8.GetBytes(header);
                requestStream.Write(headerbytes, 0, headerbytes.Length);
                while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) != 0)
                    requestStream.Write(buffer, 0, bytesRead);

                requestStream.Write(endbytes, 0, endbytes.Length);
            }

            using (var response = (HttpWebResponse) request.GetResponse())
            {
                using (var sr = new StreamReader(response.GetResponseStream()))
                {
                    result = sr.ReadToEnd();
                }
            }

            switch (outDataType)
            {
                case WebRequestDataTypes.XML:
                    return XmlHelper.DeserializeObject<T>(result);
                default:
                    return JsonConvert.DeserializeObject<T>(result);
            }
        }

        /// <summary>
        ///     带证书的post
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url"></param>
        /// <param name="obj"></param>
        /// <param name="cer"></param>
        /// <param name="result"></param>
        /// <param name="serializeStrFunc"></param>
        /// <param name="inputDataType"></param>
        /// <param name="outDataType"></param>
        /// <returns></returns>
        public T HttpPost<T>(string url, object obj, X509Certificate2 cer, out string result,
            Func<string, string> serializeStrFunc = null, WebRequestDataTypes inputDataType = WebRequestDataTypes.JSON,
            WebRequestDataTypes outDataType = WebRequestDataTypes.JSON) where T : class
        {
            string postStr = null;
            switch (inputDataType)
            {
                case WebRequestDataTypes.XML:
                    postStr = XmlHelper.SerializeObject(obj);
                    break;
                default:
                    postStr = JsonConvert.SerializeObject(obj);
                    break;
            }

            if (serializeStrFunc != null)
                postStr = serializeStrFunc(postStr);
            WeChatPayHelper.LoggerAction?.Invoke("api", "postStrs:" + postStr);
            result = HttpPost(url, postStr, cer);
            WeChatPayHelper.LoggerAction?.Invoke("api", "result:" + result);
            switch (outDataType)
            {
                case WebRequestDataTypes.XML:
                    return XmlHelper.DeserializeObject<T>(result);
                default:
                    return JsonConvert.DeserializeObject<T>(result);
            }
        }

        /// <summary>
        ///     post请求返回html
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public string HttpPost(string url, Dictionary<string, string> postdata)
        {
            string postDataStr = null;
            if (postdata != null && postdata.Count > 0)
                postDataStr = string.Join("&", postdata.Select(it => it.Key + "=" + it.Value));
            return HttpPost(url, postdata);
        }

        /// <summary>
        ///     get请求获取返回的html
        /// </summary>
        /// <param name="url">无参URL</param>
        /// <param name="querydata">参数</param>
        /// <returns></returns>
        public string HttpGet(string url, Dictionary<string, string> querydata)
        {
            if (querydata != null && querydata.Count > 0)
                url += "?" + string.Join("&", querydata.Select(it => it.Key + "=" + it.Value));
            return HttpGet(url);
        }

        /// <summary>
        ///     get请求获取返回的html
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public string HttpGet(string url)
        {
            var request = CreateWebRequest(url);
            request.Method = "GET";
            // response.Cookies = cookie.GetCookies(response.ResponseUri);
            using (var response = (HttpWebResponse) request.GetResponse())
            {
                using (var sr = new StreamReader(response.GetResponseStream(), Encoding.GetEncoding("utf-8")))
                {
                    var result = sr.ReadToEnd();
                    return result;
                }
            }
        }

        protected HttpWebRequest CreateWebRequest(string url)
        {
            var request = (HttpWebRequest) WebRequest.Create(url);
            if (ProxyAddress != null && ProxyAddress != "")
            {
                var proxy = new WebProxy();
                proxy.Address = new Uri(ProxyAddress); //按配置文件创建Proxy 地置
                if (proxy.Address != null) //如果地址为空，则不需要代理服务器
                {
                    proxy.Credentials = new NetworkCredential(ProxyUser, ProxyKey); //从配置封装参数中创建
                    request.Proxy = proxy;
                }
            }

            request.ContentType = ContentType;
            if (cookie != null)
                request.CookieContainer = Cookie;
            if (string.IsNullOrEmpty(AcceptLanguage))
            {
                var myWebHeaderCollection = request.Headers;
                myWebHeaderCollection.Add("Accept-Language", AcceptLanguage);
            }

            request.Accept = Accept;
            request.UseDefaultCredentials = true;
            request.UserAgent = UserAgent;
            request.Timeout = TimeOut;
            request.AllowAutoRedirect = AllowAutoRedirect;
            SetCertificatePolicy();
            return request;
        }

        /// <summary>
        ///     POST文件
        /// </summary>
        /// <param name="url"></param>
        /// <param name="file">文件路径</param>
        /// <param name="postdata"></param>
        /// <returns></returns>
        public string HttpUploadFile(string url, string file, Dictionary<string, string> postdata)
        {
            return HttpUploadFile(url, file, postdata, Encoding.UTF8);
        }

        /// <summary>
        ///     POST文件
        /// </summary>
        /// <param name="url"></param>
        /// <param name="file">文件路径</param>
        /// <param name="postdata">参数</param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public string HttpUploadFile(string url, string file, Dictionary<string, string> postdata, Encoding encoding)
        {
            return HttpUploadFile(url, new[] {file}, postdata, encoding);
        }

        /// <summary>
        ///     POST文件
        /// </summary>
        /// <param name="url"></param>
        /// <param name="files">文件路径</param>
        /// <param name="postdata">参数</param>
        /// <returns></returns>
        public string HttpUploadFile(string url, string[] files, Dictionary<string, string> postdata)
        {
            return HttpUploadFile(url, files, postdata, Encoding.UTF8);
        }

        /// <summary>
        ///     POST文件
        /// </summary>
        /// <param name="url"></param>
        /// <param name="files">文件路径</param>
        /// <param name="postdata">参数</param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public string HttpUploadFile(string url, string[] files, Dictionary<string, string> postdata, Encoding encoding)
        {
            var boundary = "---------------------------" + DateTime.Now.Ticks.ToString("x");
            var boundarybytes = Encoding.ASCII.GetBytes("\r\n--" + boundary + "\r\n");
            var endbytes = Encoding.ASCII.GetBytes("\r\n--" + boundary + "--\r\n");


            var request = CreateWebRequest(url);
            request.ContentType = "multipart/form-data; boundary=" + boundary;
            request.Method = "POST";
            request.KeepAlive = true;
            ;
            request.AllowAutoRedirect = AllowAutoRedirect;
            if (Cookie != null)
                request.CookieContainer = Cookie;
            request.Credentials = CredentialCache.DefaultCredentials;

            using (var stream = request.GetRequestStream())
            {
                //1.1 key/value
                var formdataTemplate = "Content-Disposition: form-data; name=\"{0}\"\r\n\r\n{1}";
                if (postdata != null)
                    foreach (var key in postdata.Keys)
                    {
                        stream.Write(boundarybytes, 0, boundarybytes.Length);
                        var formitem = string.Format(formdataTemplate, key, postdata[key]);
                        var formitembytes = encoding.GetBytes(formitem);
                        stream.Write(formitembytes, 0, formitembytes.Length);
                    }

                //1.2 file
                var headerTemplate =
                    "Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"\r\nContent-Type: application/octet-stream\r\n\r\n";
                var buffer = new byte[4096];
                var bytesRead = 0;
                for (var i = 0; i < files.Length; i++)
                {
                    stream.Write(boundarybytes, 0, boundarybytes.Length);
                    var header = string.Format(headerTemplate, "file" + i, Path.GetFileName(files[i]));
                    var headerbytes = encoding.GetBytes(header);
                    stream.Write(headerbytes, 0, headerbytes.Length);
                    using (var fileStream = new FileStream(files[i], FileMode.Open, FileAccess.Read))
                    {
                        while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) != 0)
                            stream.Write(buffer, 0, bytesRead);
                    }
                }

                //1.3 form end
                stream.Write(endbytes, 0, endbytes.Length);
            }

            //2.WebResponse
            var response = (HttpWebResponse) request.GetResponse();
            using (var stream = new StreamReader(response.GetResponseStream()))
            {
                return stream.ReadToEnd();
            }
        }


        /// <summary>
        ///     获得响应中的图像
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public Stream GetResponseImage(string url)
        {
            Stream stream = null;
            try
            {
                var request = CreateWebRequest(url);
                request.KeepAlive = true;
                request.Method = "GET";
                var res = (HttpWebResponse) request.GetResponse();
                stream = res.GetResponseStream();
                return stream;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        ///     正则获取匹配的第一个值
        /// </summary>
        /// <param name="html"></param>
        /// <param name="pattern"></param>
        /// <returns></returns>
        private string GetStringByRegex(string html, string pattern)
        {
            var re = new Regex(pattern, RegexOptions.IgnoreCase);
            var matchs = re.Matches(html);
            if (matchs.Count > 0)
                return matchs[0].Groups[1].Value;
            return "";
        }

        /// <summary>
        ///     正则验证返回的response是否正确
        /// </summary>
        /// <param name="html">Html内容</param>
        /// <param name="pattern">正则表达式</param>
        /// <returns></returns>
        private bool VerifyResponseHtml(string html, string pattern)
        {
            var re = new Regex(pattern);
            return re.IsMatch(html);
        }

        //注册证书验证回调事件，在请求之前注册
        private void SetCertificatePolicy()
        {
            ServicePointManager.ServerCertificateValidationCallback
                += RemoteCertificateValidate;
        }

        /// <summary>
        ///     远程证书验证，固定返回true
        /// </summary>
        private static bool RemoteCertificateValidate(object sender, X509Certificate cert,
            X509Chain chain, SslPolicyErrors error)
        {
            return true;
        }
    }
}
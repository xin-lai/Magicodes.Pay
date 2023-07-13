using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;

namespace Magicodes.Pay.Icbcpay
{
    public class WebUtils
    {
        private static readonly string DefaultUserAgent = "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; SV1; .NET CLR 1.1.4322; .NET CLR 2.0.50727)";

        public static String getHttpResponseStr(string url, Dictionary<string, string> parameters, String charset)
        {
            HttpWebRequest request = null;
            String urlStr = buildGetUrl(url, parameters, charset);
            //HTTPSQ请求:既支持http请求也支持https请求      
            request = WebRequest.Create(urlStr) as HttpWebRequest;
            //HttpVersion.Version10:defines a version instance for http 1.0
            //HttpVersion.Version11:defines a version instance for http 1.1，用1.1报网络异常、不通。
            request.ProtocolVersion = HttpVersion.Version10;
            request.Method = "GET";
            request.Timeout = 8000;
            request.ReadWriteTimeout = 30000;
            var response = request.GetResponse() as HttpWebResponse;
            var responseString = String.Empty;
            var httpStatusCode = (int)response.StatusCode;
            if (httpStatusCode == 200)
            {
                responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
                return responseString;
            }
            else
            {
                throw new Exception("response status code is not valid. status code: " + httpStatusCode);
            }
        }

        public static HttpWebResponse CreatePostHttpResponse(string url, IDictionary<string, string> parameters, String charset)
        {
            HttpWebRequest request = null;
            //HTTPSQ请求:既支持http请求也支持https请求            
            request = WebRequest.Create(url) as HttpWebRequest;
            request.ProtocolVersion = HttpVersion.Version10;
            request.Method = "POST";
            request.Timeout = 8000;
            request.ReadWriteTimeout = 30000;
            request.ContentType = "application/x-www-form-urlencoded;charset=GBK";
            request.UserAgent = DefaultUserAgent;
            Encoding encode = Encoding.GetEncoding(charset);
            //如果需要POST数据   
            if (!(parameters == null || parameters.Count == 0))
            {
                StringBuilder buffer = new StringBuilder();
                int i = 0;
                foreach (string key in parameters.Keys)
                {
                    if (parameters[key] == null || parameters[key].Equals(""))
                    {
                        continue;
                    }
                    else
                    {
                        if (i > 0)
                        {
                            buffer.AppendFormat("&{0}={1}", key, HttpUtility.UrlEncode(parameters[key], Encoding.GetEncoding(charset)));
                        }
                        else
                        {
                            buffer.AppendFormat("{0}={1}", key, HttpUtility.UrlEncode(parameters[key], Encoding.GetEncoding(charset)));
                        }
                        i++;
                    }

                }
                //如果用户输入带加号的参数值，需要转义
                byte[] data = encode.GetBytes(buffer.ToString());//.Replace("+","%2B"));
                using (Stream stream = request.GetRequestStream())
                {
                    stream.Write(data, 0, data.Length);
                }

            }
            return request.GetResponse() as HttpWebResponse;
        }
        public static String GetResponseJson(string url, IDictionary<string, string> parameters, String charset)
        {
            var test = JsonConvert.SerializeObject(parameters);
            HttpWebResponse response = CreatePostHttpResponse(url, parameters, charset);
            var httpStatusCode = (int)response.StatusCode;
            if (httpStatusCode == 200)
            {
                Stream stream = response.GetResponseStream();
                StreamReader sreader = new StreamReader(stream, Encoding.GetEncoding(charset));
                String responseStr = sreader.ReadToEnd();
                Console.WriteLine(responseStr);
                return responseStr;
            }
            else
            {
                throw new Exception("response status code is not valid. status code: " + httpStatusCode);
            }

        }

        public static String buildOrderedSignStr(String path, Dictionary<String, String> param)
        {
            Dictionary<String, String> tmp = new Dictionary<string, string>();
            tmp = param;
            var orderList = tmp.OrderBy(s => s.Key);
            StringBuilder sb = new StringBuilder(path);
            sb.Append("?");
            Boolean hasParam = false;
            foreach (var s in orderList)
            {
                String name = s.Key;
                String value = s.Value;
                if (value == null || name == null || value.Equals(""))
                {
                    continue;
                }
                else
                {
                    if (hasParam)
                    {
                        sb.Append("&");
                    }
                    else
                    {
                        hasParam = true;
                    }
                    sb.Append(name).Append("=").Append(value);
                }
            }
            return sb.ToString();
        }
        public static String buildGetUrl(String strUrl, Dictionary<String, String> param, String charset)
        {
            if (param == null)
            {
                return strUrl;
            }

            StringBuilder sb = new StringBuilder(strUrl);
            try
            {
                if (new Uri(strUrl).Query == null || new Uri(strUrl).Query.Equals(""))
                {
                    if (!strUrl.EndsWith("?"))
                    {
                        sb.Append('?');
                    }
                }
            }
            catch (UriFormatException e)
            {
                throw new Exception("url exception. url: " + strUrl, e);
            }

            Dictionary<String, String> tmp = new Dictionary<string, string>();
            tmp = param;
            Boolean hasParam = false;

            Boolean shouldAddAnd = strUrl.Contains("?") ? !strUrl.EndsWith("&") : false;

            foreach (var s in tmp)
            {
                String name = s.Key;
                String value = s.Value;
                // 忽略参数名或参数值为空的参数

                if (value == null || name == null || value.Equals(""))
                {
                    continue;
                }
                else
                {
                    if (hasParam)
                    {
                        sb.Append("&");
                    }
                    else
                    {
                        if (shouldAddAnd)
                        {
                            sb.Append("&");
                        }
                        hasParam = true;
                    }
                    sb.Append(name).Append("=").Append(HttpUtility.UrlEncode(value, Encoding.GetEncoding(charset)));
                }

            }

            return sb.ToString();
        }

        public static String buildForm(String baseUrl, Dictionary<String, String> parameters)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("<form name=\"auto_submit_form\" method=\"post\" action=\"");
            sb.Append(baseUrl);
            sb.Append("\">\n");
            sb.Append(buildHiddenFields(parameters));

            sb.Append("<input type=\"submit\" value=\"立刻提交\" style=\"display:none\" >\n");
            sb.Append("</form>\n");
            sb.Append("<script>document.forms[0].submit();</script>");
            String form = sb.ToString();
            return form;
        }

        private static String buildHiddenFields(Dictionary<String, String> parameters)
        {
            if (parameters == null || parameters.Count == 0)
            {
                return "";
            }
            StringBuilder sb = new StringBuilder();

            foreach (var s in parameters)
            {
                String key = s.Key;
                String value = s.Value;
                // 除去参数中的空值
                if (key == null || value == null)
                {
                    continue;
                }
                sb.Append(buildHiddenField(key, value));
            }

            String result = sb.ToString();
            return result;
        }

        private static String buildHiddenField(String key, String value)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("<input type=\"hidden\" name=\"");
            sb.Append(key);

            sb.Append("\" value=\"");
            // 转义双引号
            String a = value.Replace("\"", "&quot;");
            sb.Append(a).Append("\">\n");
            return sb.ToString();
        }


    }
}

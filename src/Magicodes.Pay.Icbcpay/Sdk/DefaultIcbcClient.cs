using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Magicodes.Pay.Icbcpay
{
    public class DefaultIcbcClient
    {
        protected String appId;
        protected String privateKey;
        protected String signType = IcbcConstants.SIGN_TYPE_RSA;
        protected String charset = IcbcConstants.CHARSET_UTF8;
        protected String format = IcbcConstants.FORMAT_JSON;
        protected String icbcPulicKey;
        protected String encryptKey;
        protected String encryptType;
        protected String ca;
        protected String password;

        public DefaultIcbcClient(String appId, String signType, String privateKey, String charset, String format,
            String icbcPulicKey, String encryptType, String encryptKey, String ca, String password)
        {
            this.appId = appId;
            this.signType = signType;
            this.privateKey = privateKey;
            this.charset = charset;
            this.format = format;
            this.icbcPulicKey = icbcPulicKey;
            this.encryptType = encryptType;
            this.encryptKey = encryptKey;
            this.password = password;

            if (ca != null && !String.Equals(ca, ""))
            {
                // 去除签名数据及证书数据中的空格
                ca = Regex.Replace(ca, @"\s", "");
            }
            this.ca = ca;
            if (signType == IcbcConstants.SIGN_TYPE_CA)
            {
                if (privateKey == null || privateKey.Equals("") || password.Equals("") || password == null || this.ca == null || this.ca.Equals(""))
                {
                    throw new Exception("CA sign Exception!");
                }
            }
        }

        public DefaultIcbcClient(String appId, String privateKey, String icbcPulicKey)
            : this(appId, IcbcConstants.SIGN_TYPE_RSA, privateKey, IcbcConstants.CHARSET_UTF8, IcbcConstants.FORMAT_JSON,
                icbcPulicKey, null, null, null, null)
        {

        }
        public DefaultIcbcClient(String appId, String signType, String privateKey, String icbcPulicKey)
            : this(appId, signType, privateKey, IcbcConstants.CHARSET_UTF8, IcbcConstants.FORMAT_JSON,
                icbcPulicKey, null, null, null, null)
        {

        }
        public DefaultIcbcClient(String appId, String privateKey, String icbcPulicKey, String ca, String password)
            : this(appId, IcbcConstants.SIGN_TYPE_CA, privateKey, IcbcConstants.CHARSET_UTF8, IcbcConstants.FORMAT_JSON,
                icbcPulicKey, null, null, ca, password)
        {

        }

        public IcbcResponse execute<T>(AbstractIcbcRequest<T> request) where T : IcbcResponse
        {
            String uuid = Guid.NewGuid().ToString();
            String msgId = uuid.Replace("-", "");
            return execute(request, msgId);
        }

        public IcbcResponse execute<T>(AbstractIcbcRequest<T> request, String msgId) where T : IcbcResponse
        {
            return execute(request, msgId, "");
        }

        public IcbcResponse execute<T>(AbstractIcbcRequest<T> request, String msgId, String appAuthToken) where T : IcbcResponse
        {
            Dictionary<String, String> param = prepareParams(request, msgId, appAuthToken);
            String respStr = null;

            if (request.getMethod().Equals("GET"))
            {
                respStr = WebUtils.getHttpResponseStr(request.getServiceUrl(), param, charset);
            }
            else if (request.getMethod().Equals("POST"))
            {
                respStr = WebUtils.GetResponseJson(request.getServiceUrl(), param, charset);
            }
            else
            {
                throw new Exception("only support GET or POST, method: " + request.getMethod());

            }

            IcbcResponse response = parseJsonWithIcbcSign(request, respStr);
            if (response == null)
            {
                throw new Exception("response is null.");
            }
            return response;
        }

        protected Dictionary<String, String> prepareParams<T>(AbstractIcbcRequest<T> request, String msgId, String appAuthToken) where T : IcbcResponse
        {
            String bizContentStr = buildBizContentStr(request);
            String path = String.Empty;
            try
            {
                path = new Uri(request.getServiceUrl()).LocalPath;
            }
            catch (UriFormatException e)
            {
                Console.WriteLine(e.Message);
            }
            Dictionary<String, String> param = new Dictionary<String, String>();

            Dictionary<String, String> extraParams = request.getExtraParams();

            if (extraParams != null)
            {
                param = param.Union(extraParams).ToDictionary(k => k.Key, v => v.Value);
            }

            param.Add(IcbcConstants.APP_ID, appId);
            param.Add(IcbcConstants.SIGN_TYPE, signType);
            param.Add(IcbcConstants.CHARSET, charset);
            param.Add(IcbcConstants.FORMAT, format);
            param.Add(IcbcConstants.CA, ca);
            param.Add(IcbcConstants.APP_AUTH_TOKEN, appAuthToken);
            param.Add(IcbcConstants.MSG_ID, msgId);
            param.Add(IcbcConstants.TIMESTAMP, DateTime.Now.ToString(IcbcConstants.DATE_TIME_FORMAT));

            if (request.isNeedEncrypt())
            {
                if (encryptType == null || encryptKey == null)
                {
                    throw new ArgumentNullException("request need be encrypted, encrypt type and encrypt key can not be null.");
                }
                if (bizContentStr != null)
                {
                    param.Add(IcbcConstants.ENCRYPT_TYPE, encryptType);
                    param.Add(IcbcConstants.BIZ_CONTENT_KEY, IcbcSignature.EncryptContent(bizContentStr, encryptType, encryptKey, charset));
                }
            }
            else
            {
                param.Add(IcbcConstants.BIZ_CONTENT_KEY, bizContentStr);
            }
            String strToSign = WebUtils.buildOrderedSignStr(path, param);
            String signedStr = IcbcSignature.sign(strToSign, signType, privateKey, charset, password);
            if (signedStr.Length < 3)
                throw new Exception("sign Exception!");

            param.Add(IcbcConstants.SIGN, signedStr);
            //if (this.signtype == icbcconstants.sign_type_ca)
            //{
            //    param.remove(icbcconstants.ca);
            //    this.ca = this.ca.replace("+", "%2b");
            //    this.ca = httputility.urlencode(this.ca,encoding.getencoding(charset));
            //    param.add(icbcconstants.ca, this.ca);
            //}
            return param;
        }

        protected String buildBizContentStr<T>(AbstractIcbcRequest<T> request) where T : IcbcResponse
        {
            if (request.getBizContent() == null)
            {
                return null;
            }
            if (String.Equals(IcbcConstants.FORMAT_JSON, format))
            {
                String json = "";
                //DataContractJsonSerializer dataSerializer = new DataContractJsonSerializer(request.getBizContentClass());
                //MemoryStream msObj = new MemoryStream();
                //dataSerializer.WriteObject(msObj,request.getBizContent());
                //msObj.Position = 0;
                //StreamReader sr = new StreamReader(msObj, Encoding.GetEncoding("utf-8"));
                //String json = sr.ReadToEnd();
                //sr.Close();
                //msObj.Close();
                //using (MemoryStream ms = new MemoryStream())
                //{
                //    dataSerializer.WriteObject(ms, request.getBizContent());
                //    json = Encoding.GetEncoding("UTF-8").GetString(ms.ToArray());
                //}
                BizContent bizContent = request.getBizContent();
                json = JsonConvert.SerializeObject(bizContent);
                return json;

                ////String json=Newtonsoft.Json.JsonConvert.SerializeObject(request.getBizContent());


                ////return json;
            }
            return null;
        }

        private IcbcResponse parseJsonWithIcbcSign<T>(AbstractIcbcRequest<T> request, String respStr) where T : IcbcResponse
        {
            String respBizContentStr = String.Empty;
            String sign = String.Empty;
            int indexOfRootStart = respStr.IndexOf(IcbcConstants.RESPONSE_BIZ_CONTENT)
                    + IcbcConstants.RESPONSE_BIZ_CONTENT.Length + 2;
            int indexOfRootEnd = respStr.LastIndexOf(",");
            int indexOfSignStart = respStr.LastIndexOf(IcbcConstants.SIGN) + IcbcConstants.SIGN.Length + 3;
            int indexOfSignEnd = respStr.LastIndexOf("\"");
            respBizContentStr = respStr.Substring(indexOfRootStart, indexOfRootEnd - indexOfRootStart);
            sign = respStr.Substring(indexOfSignStart, indexOfSignEnd - indexOfSignStart);

            Boolean passed = IcbcSignature.verify(respBizContentStr, IcbcConstants.SIGN_TYPE_RSA, icbcPulicKey, charset, sign);
            if (!passed)
            {
                throw new Exception("icbc sign verify not passed.");
            }
            if (request.isNeedEncrypt())
            {
                //解密【目前仅支持AES加解密方法】
                respBizContentStr = IcbcSignature.DecryptContent(respBizContentStr.Substring(1, respBizContentStr.Length - 1), encryptType, encryptKey, charset);

            }
            //反序列化并返回
            IcbcResponse response = null;
            //MemoryStream ms1 = new MemoryStream(Encoding.GetEncoding(charset).GetBytes(respBizContentStr));
            //using (MemoryStream ms = new MemoryStream(Encoding.GetEncoding(charset).GetBytes(respBizContentStr)))
            //{
            //    DataContractJsonSerializer deseralizer = new DataContractJsonSerializer(request.getResponseClass());
            //    response = (IcbcResponse)deseralizer.ReadObject(ms) as IcbcResponse;
            //}
            response = (IcbcResponse)JsonConvert.DeserializeObject(respBizContentStr, request.getResponseClass());
            return response;
        }

    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace Magicodes.Pay.Icbcpay
{
    public abstract class AbstractIcbcRequest<T> where T : IcbcResponse
    {
        private String serviceUrl;
        public String getServiceUrl()
        {
            return serviceUrl;
        }
        public void setServiceUrl(String value)
        {
            serviceUrl = value;
        }
        private BizContent bizContent;
        public BizContent getBizContent()
        {
            return bizContent;
        }
        public void setBizContent(BizContent value)
        {
            bizContent = value;
        }

        private Dictionary<String, String> extraParams = new Dictionary<String, String>();

        public Dictionary<String, String> getExtraParams()
        {
            return extraParams;
        }

        abstract public Type getResponseClass();
        abstract public Boolean isNeedEncrypt();
        abstract public Type getBizContentClass();
        //        abstract public Dictionary<String, String> getExtraParameters();


        /**
         * @return POST or GET
         */
        abstract public String getMethod();
    }
}

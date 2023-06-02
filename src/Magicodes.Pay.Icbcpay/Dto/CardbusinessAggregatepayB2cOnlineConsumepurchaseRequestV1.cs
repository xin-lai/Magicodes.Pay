using Magicodes.Pay.Icbcpay;
using System;
using System.Runtime.Serialization;

namespace Magicodes.Pay.Icbc.Dto
{
    public class CardbusinessAggregatepayB2cOnlineConsumepurchaseRequestV1 : AbstractIcbcRequest<IcbcResponse>
    {

        public override Type getResponseClass()
        {
            return Type.GetType("sdk_cop.B2cPassfreeAgreementSignRequestV1"); ;
        }

        public override bool isNeedEncrypt()
        {
            return false;
        }

        public override Type getBizContentClass()
        {
            return Type.GetType("sdk_cop" + ".B2cPassfreeAgreementSignRequestV1+B2cPassfreeAgreementSignRequestV1Biz", true, true);
        }

        public override string getMethod()
        {
            return "POST";
        }

        [DataContract]
        public class CardbusinessAggregatepayB2cOnlineConsumepurchaseRequestV1Biz : BizContent
        {
            /// <summary>
            /// 商户编号
            /// </summary>
            [DataMember]
            private String mer_id;
            /// <summary>
            /// 商户订单号，只能是数字、大小写字母，且在同一个商户号下唯一	
            /// </summary>
            [DataMember]
            private String out_trade_no;
            /// <summary>
            /// 支付方式，9-微信；10-支付宝；13-云闪付	
            /// </summary>
            [DataMember]
            private String pay_mode;
            /// <summary>
            /// 收单接入方式，5-APP，7-微信公众号，8-支付宝生活号，9-微信小程序	
            /// </summary>
            [DataMember]
            private String access_type;
            /// <summary>
            /// 收单产品协议编号	
            /// </summary>
            [DataMember]
            private String mer_prtcl_no;
            /// <summary>
            /// 交易日期时间，格式为yyyy-MM-dd’T’HH:mm:ss	
            /// </summary>
            [DataMember]
            private String orig_date_time;
            /// <summary>
            /// 设备号	
            /// </summary>
            [DataMember]
            private String decive_info;
            /// <summary>
            /// 商品描述，商品描述交易字段格式根据不同的应用场景按照以下格式：1）PC网站：传入浏览器打开的网站主页title名-实际商品名称 ；2）公众号：传入公众号名称-实际商品名称；3）H5：传入浏览器打开的移动网页的主页title名-实际商品名称；4）线下门店：门店品牌名-城市分店名-实际商品名称；5）APP：传入应用市场上的APP名字-实际商品名称	
            /// </summary>
            [DataMember]
            private String body;
            /// <summary>
            /// 交易币种，目前工行只支持使用人民币（001）支付	
            /// </summary>
            [DataMember]
            private String fee_type;
            /// <summary>
            /// 用户端IP	
            /// </summary>
            [DataMember]
            private String spbill_create_ip;
            /// <summary>
            /// 订单金额，单位为分	
            /// </summary>
            [DataMember]
            private String total_fee;
            /// <summary>
            /// 异步通知商户URL，端口必须为443或80	
            /// </summary>
            [DataMember]
            private String mer_url;
            /// <summary>
            /// 商户在微信开放平台注册的APPID，支付方式为微信时不能为空	
            /// </summary>
            [DataMember]
            private String shop_appid;
            /// <summary>
            /// 商户在工行API平台的APPID	
            /// </summary>
            [DataMember]
            private String icbc_appid;
            /// <summary>
            /// 第三方用户标识，商户在微信公众号内或微信小程序内接入时必送，即access_type为7或9时，上送用户在商户APPID下的唯一标识；商户通过支付宝生活号接入时不送	
            /// </summary>
            [DataMember]
            private String open_id;
            /// <summary>
            /// 第三方用户标识，商户在支付宝生活号接入时必送，即access_type为8时，上送用户的唯一标识；商户通过微信公众号内或微信小程序接入时不送	
            /// </summary>
            [DataMember]
            private String union_id;
            /// <summary>
            /// 商户账号，商户入账账号，只能交易时指定。（商户付给银行手续费的账户，可以在开户的时候指定，也可以用交易指定方式；用交易指定方式则使用此商户账号）目前暂不支持	
            /// </summary>
            [DataMember]
            private String mer_acct;
            /// <summary>
            /// 订单失效时间，单位为秒，建议大于60秒	
            /// </summary>
            [DataMember]
            private String expire_time;
            /// <summary>
            /// 附加数据，在查询API和支付通知中原样返回，该字段主要用于商户携带订单的自定义数据	
            /// </summary>
            [DataMember]
            private String attach;
            /// <summary>
            /// 通知类型，表示在交易处理完成后把交易结果通知商户的处理模式。 取值“HS”：在交易完成后将通知信息，主动发送给商户，发送地址为mer_url指定地址； 取值“AG”：在交易完成后不通知商户	
            /// </summary>
            [DataMember]
            private String notify_type;
            /// <summary>
            /// 结果发送类型，通知方式为HS时有效。取值“0”：无论支付成功或者失败，银行都向商户发送交易通知信息；取值“1”，银行只向商户发送交易成功的通知信息。默认是”0”	
            /// </summary>
            [DataMember]
            private String result_type;
            /// <summary>
            /// 支付方式限定，上送”no_credit“表示不支持信用卡支付；上送“no_balance”表示仅支持银行卡支付；不上送或上送空表示无限制	
            /// </summary>
            [DataMember]
            private String pay_limit;
            /// <summary>
            /// 订单附加信息	
            /// </summary>
            [DataMember]
            private String order_apd_inf;
            /// <summary>
            /// 商品详细描述，对于使用单品优惠的商户，该字段必须按照规范上传。微信与支付宝的规范不同，请根据支付方式对应相应的规范上送，详细内容参考文末说明	
            /// </summary>
            [DataMember]
            private String detail;
            /// <summary>
            /// 支付成功回显页面，支付成功后，跳转至该页面显示。当access_type=5且pay_mode=10才有效	
            /// </summary>
            [DataMember]
            private String return_url;
            /// <summary>
            /// 用户付款中途退出返回商户网站的地址（仅对浏览器内支付时有效）当access_type=5且pay_mode=10才有效
            /// </summary>
            [DataMember]
            private String quit_url; 


            public String getMer_id()
            {
                return mer_id;
            }
            public void setMer_id(String input)
            {
                this.mer_id = input;
            }

            public String getOut_trade_no()
            {
                return out_trade_no;
            }
            public void setOut_trade_no(String input)
            {
                this.out_trade_no = input;
            }

            public String getPay_mode()
            {
                return pay_mode;
            }
            public void setPay_mode(String input)
            {
                this.pay_mode = input;
            }

            public String getAccess_type()
            {
                return access_type;
            }
            public void setAccess_type(String input)
            {
                this.access_type = input;
            }

            public String getMer_prtcl_no()
            {
                return mer_prtcl_no;
            }
            public void setMer_prtcl_no(String input)
            {
                this.mer_prtcl_no = input;
            }

            public String getOrig_date_time()
            {
                return orig_date_time;
            }
            public void setOrig_date_time(String input)
            {
                this.orig_date_time = input;
            }
            public String getDecive_info()
            {
                return decive_info;
            }
            public void setDecive_info(String input)
            {
                this.decive_info = input;
            }
            public String getBody()
            {
                return body;
            }
            public void setBody(String input)
            {
                this.body = input;
            }
            public String getFee_type()
            {
                return fee_type;
            }
            public void setFee_type(String input)
            {
                this.fee_type = input;
            }
            public String getSpbill_create_ip()
            {
                return spbill_create_ip;
            }
            public void setSpbill_create_ip(String input)
            {
                this.spbill_create_ip = input;
            }
            public String getTotal_fee()
            {
                return total_fee;
            }
            public void setTotal_fee(String input)
            {
                this.total_fee = input;
            }
            public String getMer_url()
            {
                return mer_url;
            }
            public void setMer_url(String input)
            {
                this.mer_url = input;
            }
            public String getShop_appid()
            {
                return shop_appid;
            }
            public void setShop_appid(String input)
            {
                this.shop_appid = input;
            }
            public String getIcbc_appid()
            {
                return icbc_appid;
            }
            public void setIcbc_appid(String input)
            {
                this.icbc_appid = input;
            }
            public String getOpen_id()
            {
                return open_id;
            }
            public void setOpen_id(String input)
            {
                this.open_id = input;
            }
            public String getUnion_id()
            {
                return union_id;
            }
            public void setUnion_id(String input)
            {
                this.union_id = input;
            }
            public String getMer_acct()
            {
                return mer_acct;
            }
            public void setMer_acct(String input)
            {
                this.mer_acct = input;
            }
            public String getExpire_time()
            {
                return expire_time;
            }
            public void setExpire_time(String input)
            {
                this.expire_time = input;
            }
            
            public String getAttach()
            {
                return attach;
            }
            public void setAttach(String input)
            {
                this.attach = input;
            }
            public String getNotify_type()
            {
                return notify_type;
            }
            public void setNotify_type(String input)
            {
                this.notify_type = input;
            }
            public String getResult_type()
            {
                return result_type;
            }
            public void setResult_type(String input)
            {
                this.result_type = input;
            }
            public String getPay_limit()
            {
                return pay_limit;
            }
            public void setPay_limit(String input)
            {
                this.pay_limit = input;
            }
            public String getOrder_apd_inf()
            {
                return order_apd_inf;
            }
            public void setOrder_apd_inf(String input)
            {
                this.order_apd_inf = input;
            }
            public String getDetail()
            {
                return detail;
            }
            public void setDetail(String input)
            {
                this.detail = input;
            }
            public String getReturn_url()
            {
                return return_url;
            }
            public void setRreturn_url(String input)
            {
                this.return_url = input;
            }
            public String getQuit_url()
            {
                return quit_url;
            }
            public void setQuit_url(String input)
            {
                this.quit_url = input;
            }
        }
    }
}

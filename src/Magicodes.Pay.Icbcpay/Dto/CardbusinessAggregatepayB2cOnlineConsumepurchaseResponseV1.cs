using Magicodes.Pay.Icbcpay; 
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Magicodes.Pay.Icbc.Dto
{

    public class CardbusinessAggregatepayB2cOnlineConsumepurchaseResponseV1 : IcbcResponse
    {
        /// <summary>
        /// 工行订单号
        /// </summary>
        [DataMember]
        private String order_id;

        /// <summary>
        /// 订单总金额，一笔订单一个，以分为单位。不可以为零，必需符合金额标准	
        /// </summary>
        [DataMember]
        private String total_amt;

        /// <summary>
        /// 商户系统订单号，原样返回	
        /// </summary>
        [DataMember]
        private string out_trade_no;
        /// <summary>
        /// 支付完成时间，格式为：yyyyMMdd	
        /// </summary>
        [DataMember]
        private string pay_time;
        /// <summary>
        /// 商户编号	
        /// </summary>
        [DataMember]
        private string mer_id;
        /// <summary>
        /// 支付方式，1-刷卡支付；2-工行快捷支付；3-网银支付；4-新型无卡；5-简单无卡；6-银联快捷支付；7-3D支付；8-ApplePay；9-微信；10-支付宝；11-非3D支付；12-银联系扫码付；13-云闪付	
        /// </summary>
        [DataMember]
        private string pay_mode;
        /// <summary>
        /// 收单接入方式，5-APP，7-微信公众号，8-支付宝生活号，9-微信小程序	
        /// </summary>
        [DataMember]
        private string access_type;
        /// <summary>
        /// 卡种，90-VISA外卡、91-MASTER外卡、92-运通外卡、93-大来外卡、94-JCB外卡	
        /// </summary>
        [DataMember]
        private string card_kind;
        /// <summary>
        /// 支付方式为微信时返回，交易类型，JSAPI ：公众号支付、小程序支付；APP：APP支付；	
        /// </summary>
        [DataMember]
        private string trade_type;
        /// <summary>
        /// 支付方式为微信时返回，微信数据包，用于之后唤起微信支付。详细内容请参考微信支付开放平台接口	
        /// </summary>
        [DataMember]
        private string wx_data_package;
        /// <summary>
        /// 支付方式为支付宝时返回，支付宝数据包，用于之后唤起支付宝支付。详细内容请参考支付宝开放平台接口	
        /// </summary>
        [DataMember]
        private string zfb_data_package;
        /// <summary>
        /// 支付方式为云闪付时返回，云闪付受理订单号，用于之后进行银联云闪付支付。详细内容请参考银联开放平台的手机支付控件接口	
        /// </summary>
        [DataMember]
        private string union_data_package;
        /// <summary>
        /// 第三方报错时返回的报错码	
        /// </summary>
        [DataMember]
        private string third_party_return_code;
        /// <summary>
        /// 第三方报错时返回的报错信息

        /// </summary>
        [DataMember]
        private string third_party_return_msg;

        public String getOrderId()
        {
            return order_id;
        }

        public void setOrderId(String orderId)
        {
            this.order_id = orderId;
        }

        public String getTotalAmt()
        {
            return total_amt;
        }

        public void setTotalAmt(String totalAmt)
        {
            this.total_amt = totalAmt;
        }

        public String getPayTime()
        {
            return pay_time;
        }

        public void setPayTime(String payTime)
        {
            this.pay_time = payTime;
        }

        public String getOutTradeNo()
        {
            return out_trade_no;
        }

        public void setOutTradeNo(String outTradeNo)
        {
            this.out_trade_no = outTradeNo;
        }

        public String getMerId()
        {
            return mer_id;
        }

        public void setMerId(String merId)
        {
            this.mer_id = merId;
        }

        public String getPayMode()
        {
            return pay_mode;
        }

        public void setPayMode(String payMode)
        {
            this.pay_mode = payMode;
        }

        public String getAccessType()
        {
            return access_type;
        }

        public void setAccessType(String accessType)
        {
            this.access_type = accessType;
        }

        public String getCardKind()
        {
            return card_kind;
        }

        public void setCardKind(String cardKind)
        {
            this.card_kind = cardKind;
        }

        public String getTradeType()
        {
            return trade_type;
        }

        public void setTradeType(String tradeType)
        {
            this.trade_type = tradeType;
        }

        public String getWxDataPackage()
        {
            return wx_data_package;
        }

        public void setWxDataPackage(String wxDataPackage)
        {
            this.wx_data_package = wxDataPackage;
        }

 
    }
}

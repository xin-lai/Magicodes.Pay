using Magicodes.Pay.Icbc.Dto;
using Magicodes.Pay.Icbcpay;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Magicodes.Pay.Icbc
{
    public class IcbcpayAppService : IIcbcpayAppService
    {
        private IIcbcpaySettings IcbcSettings { get; set; }

        public IcbcpayAppService()
        {
        }

        public static Action<string, string> LoggerAction { get; set; }
        public static Func<IIcbcpaySettings> GetPayConfigFunc { get; set; }

        /// <summary>
        /// b2c聚合支付接口
        /// </summary>

        public Task<B2cAggregatedPayOutput> B2cAggregatedPay(PayInput input)
        {
            IcbcSettings = GetPayConfigFunc();

            //签名类型为RSA时，需传入appid，私钥和网关公钥，签名类型使用定值IcbcConstants.SIGN_TYPE_RSA，其他参数使用缺省值
            DefaultIcbcClient client = new DefaultIcbcClient(IcbcSettings.APP_ID, IcbcConstants.SIGN_TYPE_RSA2, IcbcSettings.MY_PRIVATE_KEY, IcbcSettings.APIGW_PUBLIC_KEY);
            CardbusinessAggregatepayB2cOnlineConsumepurchaseRequestV1 request = new CardbusinessAggregatepayB2cOnlineConsumepurchaseRequestV1();
            //根据测试环境和生产环境替换相应ip和端口
            request.setServiceUrl("https://gw.open.Icbc.com.cn/api/cardbusiness/aggregatepay/b2c/online/consumepurchase/V1");
            CardbusinessAggregatepayB2cOnlineConsumepurchaseRequestV1.CardbusinessAggregatepayB2cOnlineConsumepurchaseRequestV1Biz bizContent
                = new CardbusinessAggregatepayB2cOnlineConsumepurchaseRequestV1.CardbusinessAggregatepayB2cOnlineConsumepurchaseRequestV1Biz();
            request.setBizContent(bizContent);
            //请对照接口文档用bizContent.setxxx()方法对业务上送数据进行赋值
            bizContent.setMer_id(IcbcSettings.MER_ID);
            bizContent.setOut_trade_no(input.OrderNumber);
            bizContent.setPay_mode(input.PayModel);
            bizContent.setAccess_type(input.AccessType);
            bizContent.setMer_prtcl_no(IcbcSettings.MER_PRTCL_NO);
            bizContent.setOrig_date_time(DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss"));
            bizContent.setDecive_info(IcbcSettings.DECIVE_INFO);
            bizContent.setBody(input.Body);
            bizContent.setFee_type("001");
            bizContent.setSpbill_create_ip("0.0.0.0");
            bizContent.setTotal_fee(input.Amount.ToString());
            bizContent.setMer_url(IcbcSettings.NOTIFY_URL);
            bizContent.setShop_appid(IcbcSettings.SHOP_APP_ID);
            bizContent.setIcbc_appid(IcbcSettings.APP_ID);
            bizContent.setOpen_id(input.OpenId);
            bizContent.setUnion_id(input.UnionId);
            //bizContent.setMer_acct("");
            bizContent.setExpire_time(input.ValidTime);
            bizContent.setAttach(input.Remark);
            bizContent.setNotify_type("HS");
            bizContent.setResult_type("0");
            //bizContent.setPay_limit("no_credit");
            //bizContent.setOrder_apd_inf(""); 
            try
            {
               var response = (CardbusinessAggregatepayB2cOnlineConsumepurchaseResponseV1)client.execute(request, Guid.NewGuid().ToString());//msgId消息通讯唯一编号，要求每次调用独立生成，APP级唯一
                if (response.getReturnCode() == 0)
                {
                    var result = new B2cAggregatedPayOutput();
                    result.order_id = response.getOrderId();
                    result.total_amt = response.getTotalAmt();
                    result.out_trade_no = response.getOutTradeNo();
                    result.pay_time = response.getPayTime();
                    result.mer_id = response.getMerId();
                    result.pay_mode = response.getPayMode();
                    result.access_type = response.getAccessType();
                    result.card_kind = response.getCardKind();
                    result.trade_type = response.getTradeType();
                    result.wx_data_package = response.getWxDataPackage(); 
                    return Task.FromResult(result);
                }
                else
                {
                    // 失败
                    LoggerAction?.Invoke("Error", "工行支付请求参数错误（FAIL）:" + IcbcSettings);
                    LoggerAction?.Invoke("Error", "input:" + input);
                    LoggerAction?.Invoke("Error", "RetMsg:" + response.getReturnCode() + "    ErrMsg:" + response.getReturnMsg());
                    throw new Exception("工行支付请求参数错误,请检查!");
                }
            }
            catch (Exception ex)
            {
                throw new Exception("工行支付请求参数错误,请检查："+ ex.Message);

            }
        }

        public IcbcpayOutput PayNotifyHandler(Dictionary<string, string> dic)
        {
            try
            {
                IcbcSettings = GetPayConfigFunc();

                var sign = dic["sign"];
                dic.Remove("sign");
                string signStr = WebUtils.buildOrderedSignStr(dic["api"], dic);
                string signType = dic["sign_type"];
                string charset = dic["charset"];

                bool verify = false;

                if (signType.Equals(IcbcConstants.SIGN_TYPE_RSA))
                    verify = RSAClass.RSAFromPkcs8.verifySHA1(signStr, sign, IcbcSettings.APIGW_PUBLIC_KEY, charset);
                else if (signType.Equals(IcbcConstants.SIGN_TYPE_RSA2))
                    verify = RSAClass.RSAFromPkcs8.verifySHA256(signStr, sign, IcbcSettings.APIGW_PUBLIC_KEY, charset);
                else
                    throw new Exception($"工行验签失败,签名类型错误，signType:{signType}");

                //if (verify)
                //{
                    var respMap = JsonConvert.DeserializeObject<IDictionary<string, string>>(dic["biz_content"]);
                    var bizResult = new BizContentResult
                    {
                        return_code = respMap["return_code"],
                        return_msg = respMap["return_msg"],
                        msg_id = respMap["msg_id"]

                    }; 
                    var bizResultJson = JsonConvert.SerializeObject(bizResult);
                    var signDic = $"\"response_biz_content\":{bizResultJson },\"sign_type\":\"{signType}\"";
                    sign = IcbcSignature.sign(signStr, signType, IcbcSettings.MY_PRIVATE_KEY, charset);

                    var icbcpayResult = new IcbcpayResult { response_biz_content = bizResult, sign = sign, sign_type = signType };

                    var result = new IcbcpayOutput() {
                        OutTradeNo = respMap["out_trade_no"],
                        TotalFee = Convert.ToDecimal(respMap["total_amt"]),
                        TradeNo = respMap["order_id"],
                        BusinessParams = respMap["attach"],
                        SuccessResult = JsonConvert.SerializeObject(icbcpayResult)
                    };

                    return result;
                //}
                //else
                //    throw new Exception($"工行验签失败,第三方返回签名：{sign}，签名类型：{signType}, 我方组装待验签参数:{signStr}");


            }
            catch (Exception e)
            {
                throw new Exception($"工行验签未知错误：{e.Message}");
            }

        }
    }
}


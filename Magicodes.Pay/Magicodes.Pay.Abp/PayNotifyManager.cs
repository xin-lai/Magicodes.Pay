using System.Linq;
using System.Threading.Tasks;
using Abp.Dependency;
using Abp.Domain.Uow;
using Abp.Json;
using Abp.UI;
using Magicodes.Alipay;
using Magicodes.Allinpay;
using Magicodes.Pay.Abp.Callbacks;
using Magicodes.Pay.Abp.Configs;
using Magicodes.Pay.WeChat;
using Magicodes.PayNotify.Models;
using Newtonsoft.Json.Linq;

namespace Magicodes.Pay.Abp
{
    /// <summary>
    /// 支付回调通知管理器
    /// </summary>
    public class PayNotifyManager : IPayNotifyManager
    {
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly IIocResolver _iocResolver;
        private readonly IPaymentCallbackManager _paymentCallbackManager;
        private readonly PaymentConfigManager _paymentConfigManager;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="unitOfWorkManager"></param>
        /// <param name="iocResolver"></param>
        /// <param name="paymentCallbackManager"></param>
        /// <param name="paymentConfigManager"></param>
        public PayNotifyManager(IUnitOfWorkManager unitOfWorkManager, IIocResolver iocResolver, IPaymentCallbackManager paymentCallbackManager, PaymentConfigManager paymentConfigManager)
        {
            _unitOfWorkManager = unitOfWorkManager;
            _iocResolver = iocResolver;
            _paymentCallbackManager = paymentCallbackManager;
            _paymentConfigManager = paymentConfigManager;
        }

        /// <summary>
        /// 执行支付通知
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<string> ExecPayNotifyAsync(PayNotifyInput input)
        {
            async Task PayActionAsync(string outTradeNo, string transactionId, int totalFee, string customData)
            {
                //目前仅用支付参数的业务字段存储key，自定义数据在交易日志的CustomData中
                var key = customData.Contains("{") ? customData.FromJsonString<JObject>()["key"]?.ToString() : customData;
                await _paymentCallbackManager.ExecuteCallback(key, outTradeNo, transactionId, totalFee);
            }

            using (var uow = _unitOfWorkManager.Begin())
            {
                switch (input.Provider)
                {
                    // ReSharper disable once StringLiteralTypo
                    case "wechat":
                        {
                            using (var obj = _iocResolver.ResolveAsDisposable<WeChatPayApi>())
                            {
                                var api = obj.Object;
                                var result = await api.PayNotifyHandler(input.Request.Body, async (output, error) =>
                                  {
                                      //获取微信支付自定义数据
                                      if (string.IsNullOrWhiteSpace(output.Attach))
                                      {
                                          throw new UserFriendlyException("自定义参数不允许为空！");
                                      }

                                      var outTradeNo = output.OutTradeNo;
                                      var totalFee = int.Parse(output.TotalFee);
                                      await PayActionAsync(outTradeNo, output.TransactionId, totalFee, output.Attach);
                                      await uow.CompleteAsync();
                                  });
                                return result;
                            }

                        }
                    // ReSharper disable once StringLiteralTypo
                    case "alipay":
                        {
                            using (var obj = _iocResolver.ResolveAsDisposable<IAlipayAppService>())
                            {
                                var api = obj.Object;

                                var dictionary = input.Request.Form.ToDictionary(p => p.Key,
                                    p2 => p2.Value.FirstOrDefault()?.ToString());
                                //签名校验
                                if (!api.PayNotifyHandler(dictionary))
                                {
                                    throw new UserFriendlyException("支付宝支付签名错误！");
                                }

                                var outTradeNo = input.Request.Form["out_trade_no"];
                                var tradeNo = input.Request.Form["trade_no"];
                                var charset = input.Request.Form["charset"];
                                var totalFee = (int)(decimal.Parse(input.Request.Form["total_fee"]) * 100);
                                var businessParams = input.Request.Form["business_params"];
                                if (string.IsNullOrWhiteSpace(businessParams))
                                {
                                    throw new UserFriendlyException("自定义参数不允许为空！");
                                }

                                await PayActionAsync(outTradeNo, tradeNo, totalFee, businessParams);
                                await uow.CompleteAsync();
                                return "success";
                            }
                        }
                    // ReSharper disable once StringLiteralTypo
                    case "allinpay":
                        {
                            using (var obj = _iocResolver.ResolveAsDisposable<IAllinpayAppService>())
                            {
                                var api = obj.Object;
                                var dictionary = input.Request.Form.ToDictionary(p => p.Key,
                                    p2 => p2.Value.FirstOrDefault()?.ToString());
                                //签名校验
                                if (!api.PayNotifyHandler(dictionary))
                                {
                                    throw new UserFriendlyException("通联支付签名错误！");
                                }

                                // ReSharper disable once StringLiteralTypo
                                var outTradeNo = input.Request.Form["outtrxid"];
                                var tradeNo = input.Request.Form["trxid"];
                                var totalFee = (int)(decimal.Parse(input.Request.Form["trxamt"]));
                                // ReSharper disable once IdentifierTypo
                                var trxreserved = input.Request.Form["trxreserved"];
                                if (string.IsNullOrWhiteSpace(trxreserved))
                                {
                                    throw new UserFriendlyException("自定义参数不允许为空！");
                                }

                                await PayActionAsync(outTradeNo, tradeNo, totalFee, trxreserved);
                                await uow.CompleteAsync();
                                return "success";
                            }
                        }
                    default:
                        break;
                }
            }

            return null;
        }
    }
}

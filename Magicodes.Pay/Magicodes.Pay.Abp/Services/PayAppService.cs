// ======================================================================
//   
//           Copyright (C) 2018-2020 湖南心莱信息科技有限公司    
//           All rights reserved
//   
//           filename : PayAppService.cs
//           description :
//   
//           created by 雪雁 at  2018-08-06 14:40
//           Mail: wenqiang.li@xin-lai.com
//           QQ群：85318032（技术交流）
//           Blog：http://www.cnblogs.com/codelove/
//           GitHub：https://github.com/xin-lai
//           Home：http://xin-lai.com
//   
// ======================================================================

using System;
using System.Threading.Tasks;
using Abp;
using Abp.Auditing;
using Abp.Dependency;
using Abp.Json;
using Abp.Timing;
using Abp.UI;
using Castle.Core.Logging;
using Magicodes.Alipay;
using Magicodes.Allinpay;
using Magicodes.Allinpay.Dto;
using Magicodes.Pay.Abp.Callbacks;
using Magicodes.Pay.Abp.Configs;
using Magicodes.Pay.Abp.Services.Dto;
using Magicodes.Pay.Abp.TransactionLogs;
using Magicodes.Pay.WeChat;
using Magicodes.Pay.WeChat.Pay.Dto;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using AppPayInput = Magicodes.Pay.Abp.Services.Dto.AppPayInput;
using AppPayOutput = Magicodes.Pay.WeChat.Pay.Dto.AppPayOutput;

namespace Magicodes.Pay.Abp.Services
{
    /// <summary>
    ///     支付服务
    /// </summary>
    public class PayAppService : IPayAppService
    {
        private readonly IClientInfoProvider _clientInfoProvider;
        private readonly IPaymentCallbackManager _paymentCallbackManager;
        private readonly PaymentConfigManager _paymentConfigManager;
        private readonly IIocManager _iocManager;

        /// <summary>
        /// 
        /// </summary>
        public ILogger Logger { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="clientInfoProvider"></param>
        /// <param name="transactionLogHelper"></param>
        /// <param name="paymentCallbackManager"></param>
        /// <param name="iocManager"></param>
        /// <param name="paymentConfigManager"></param>
        public PayAppService(IClientInfoProvider clientInfoProvider, TransactionLogHelper transactionLogHelper, IPaymentCallbackManager paymentCallbackManager, IIocManager iocManager, PaymentConfigManager paymentConfigManager)
        {
            _clientInfoProvider = clientInfoProvider;
            _transactionLogHelper = transactionLogHelper;
            _paymentCallbackManager = paymentCallbackManager;
            _iocManager = iocManager;
            this._paymentConfigManager = paymentConfigManager;
            Logger = NullLogger.Instance;
        }

        private readonly TransactionLogHelper _transactionLogHelper;

        /// <summary>
        /// 支付
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<object> Pay(PayInput input)
        {
            Logger.Debug("准备发起支付：" + input.ToJsonString());
            object output = null;
            Exception exception = null;
            if (input.OutTradeNo == null)
            {
                input.OutTradeNo = GenerateOutTradeNo();
            }

            try
            {
                //TODO:添加客户端请求头判断,支持自动使用PC/H5/APP等支付
                switch (input.PayChannel)
                {
                    case PayChannels.WeChatPay:
                        output = await WeChatAppPay(input);
                        break;
                    case PayChannels.WeChatMiniProgram:
                        output = await WeChatMiniAppPay(input);
                        break;
                    case PayChannels.AliPay:
                        output = await AliAppPay(input);
                        break;
                    //通联支付
                    case PayChannels.AllinWeChatMiniPay:
                        output = await AllinWeChatMiniPay(input);
                        break;
                    //case PayChannels.BalancePay:
                    //    await BalancePay(input);
                    //    return null;
                    default:
                        throw new UserFriendlyException("当前不支持此种类型的支付！");
                }
            }
            catch (Exception ex)
            {
                exception = ex;
            }

            if (input.PayChannel != PayChannels.BalancePay)
            {
                //创建交易日志
                await CreateToPayTransactionInfo(input, exception);
                if (exception != null)
                {
                    Logger.Error("支付失败！", exception);
                    throw new UserFriendlyException("支付异常，请联系客服人员或稍后再试！");
                }
            }

            return output;
        }

        /// <summary>
        ///     支付宝APP支付
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        //[HttpPost("AppPay/Alipay")]
        protected async Task<string> AliAppPay(AppPayInput input)
        {
            var alipayAppService = _iocManager.Resolve<IAlipayAppService>();
            if (alipayAppService == null)
            {
                throw new UserFriendlyException("支付未开放，请联系管理员！");
            }
            var appPayInput = new Alipay.Dto.AppPayInput
            {
                Body = input.Body,
                Subject = input.Subject,
                TradeNo = input.OutTradeNo,
                PassbackParams = input.CustomData,
                TotalAmount = input.TotalAmount
            };
            try
            {
                var appPayOutput = await alipayAppService.AppPay(appPayInput);
                return appPayOutput.Response.Body;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }

        /// <summary>
        /// 通联支付
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        protected async Task<AllinpayResponse> AllinWeChatMiniPay(PayInput input)
        {
            var allinpayAppService = _iocManager.Resolve<IAllinpayAppService>();
            if (allinpayAppService == null)
            {
                throw new UserFriendlyException("支付未开放，请联系管理员！");
            }
            var weChatMiniPayInput = new WeChatMiniPayInput()
            {
                Body = input.Body,
                OrderNumber = input.OutTradeNo,
                Amount = (int)(input.TotalAmount * 100),
                OpenId = input.OpenId,
                //不通过此处传递自定义参数
                Remark = input.Key
            };
            try
            {
                var appPayOutput = await allinpayAppService.WeChatMiniPay(weChatMiniPayInput);
                return appPayOutput.Response;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }

        /// <summary>
        ///     微信APP支付
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        //[HttpPost("AppPay/WeChat")]
        protected Task<AppPayOutput> WeChatAppPay(PayInput input)
        {
            var weChatPayApi = _iocManager.Resolve<WeChatPayApi>();
            if (weChatPayApi == null)
            {
                throw new UserFriendlyException("支付未开放，请联系管理员！");
            }
            var appPayInput = new Pay.WeChat.Pay.Dto.AppPayInput
            {
                Body = input.Body,
                OutTradeNo = input.OutTradeNo,
                Attach = input.Key,
                TotalFee = input.TotalAmount,
                SpbillCreateIp = _clientInfoProvider?.ClientIpAddress
            };
            try
            {
                var appPayOutput = weChatPayApi.AppPay(appPayInput);
                return Task.FromResult(appPayOutput);
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }

        /// <summary>
        ///     微信小程序APP支付
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        //[HttpPost("AppPay/WeChat")]
        protected Task<MiniProgramPayOutput> WeChatMiniAppPay(PayInput input)
        {
            var weChatPayApi = _iocManager.Resolve<WeChatPayApi>();
            if (weChatPayApi == null)
            {
                throw new UserFriendlyException("支付未开放，请联系管理员！");
            }
            var appPayInput = new Pay.WeChat.Pay.Dto.MiniProgramPayInput()
            {
                Body = input.Body,
                OutTradeNo = input.OutTradeNo,
                Attach = input.Key,
                TotalFee = input.TotalAmount,
                SpbillCreateIp = _clientInfoProvider?.ClientIpAddress,
                OpenId = input.OpenId,
            };
            try
            {
                var appPayOutput = weChatPayApi.MiniProgramPay(appPayInput);
                return Task.FromResult(appPayOutput);
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }

        /// <summary>
        /// 余额支付
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        protected async Task BalancePay(PayInput input)
        {
            var data = JsonConvert.DeserializeObject<JObject>(input.CustomData);
            var uid = data["uid"]?.ToString();
            var log = await CreateToPayTransactionInfo(input);

            if (input.Key == "系统充值")
            {
                throw new UserFriendlyException("余额支付不支持此业务！");
            }

            var userIdentifer = UserIdentifier.Parse(uid);
            //TODO:余额支付
            //await UserManager.UpdateRechargeInfo(userIdentifer, (int)(-input.TotalAmount * 100));
            await _paymentCallbackManager.ExecuteCallback(input.Key, log.OutTradeNo, log.TransactionId, (int)(input.TotalAmount * 100));
        }

        /// <summary>
        /// 创建交易日志
        /// </summary>
        /// <returns></returns>
        private async Task<TransactionLog> CreateToPayTransactionInfo(PayInput input, Exception exception = null)
        {
            var transactionInfo = new TransactionInfo()
            {
                Amount = input.TotalAmount,
                CustomData = input.CustomData,
                OutTradeNo = input.OutTradeNo ?? GenerateOutTradeNo(),
                PayChannel = input.PayChannel,
                Subject = input.Subject,
                TransactionState = TransactionStates.NotPay,
                //TransactionId = "",
                Exception = exception
            };
            TransactionLog transactionLog = null;
            transactionLog = _transactionLogHelper.CreateTransactionLog(transactionInfo);
            await _transactionLogHelper.SaveAsync(transactionLog);
            return transactionLog;
        }

        /// <summary>
        /// 生成交易单号
        /// </summary>
        /// <returns></returns>
        private string GenerateOutTradeNo()
        {
            var code = RandomHelper.GetRandom(1000, 9999);
            return $"M{Clock.Now:yyyyMMddHHmmss}{code}";
        }
    }
}
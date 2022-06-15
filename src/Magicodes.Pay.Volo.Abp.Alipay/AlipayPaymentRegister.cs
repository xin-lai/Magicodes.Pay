using System;
using System.Linq;
using System.Threading.Tasks;
using Magicodes.Pay.Alipay;
using Magicodes.Pay.Alipay.Builder;
using Magicodes.Pay.Notify.Models;
using Magicodes.Pay.Volo.Abp.Dto;
using Magicodes.Pay.Volo.Abp.Registers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Volo.Abp;
using Volo.Abp.Json;
using Volo.Abp.Settings;

namespace Magicodes.Pay.Abp.Alipay
{
    /// <summary>
    /// 支付宝支付配置
    /// </summary>
    public class AlipayPaymentRegister : PaymentRegisterBase
    {
        public AlipayPaymentRegister(IServiceProvider serviceProvider, IJsonSerializer jsonSerializer, ILogger<PaymentRegisterBase> logger) : base(serviceProvider, jsonSerializer, logger)
        {
        }

        /// <summary>
        /// 关键字
        /// </summary>
        public override string Key { get; set; } = "Alipay";

        public override void Build(Action<string, string> logAction)
        {
            AlipayBuilder.Create()
                .WithLoggerAction(logAction)
                .RegisterGetPayConfigFunc(() => GetConfigFromConfigOrSettingsByKey<AlipaySettings>().Result).Build();
        }


        public override async Task<ExecPayNotifyOutputDto> ExecPayNotifyAsync(PayNotifyInput input)
        {
            var api = serviceProvider.GetRequiredService<IAlipayAppService>();
            {
                var dictionary = input.Request.Form.ToDictionary(p => p.Key,
                    p2 => p2.Value.FirstOrDefault()?.ToString());
                //签名校验
                if (!api.PayNotifyHandler(dictionary))
                {
                    throw new BusinessException("支付宝支付签名错误！");
                }

                var outTradeNo = input.Request.Form["out_trade_no"];
                var tradeNo = input.Request.Form["trade_no"];
                var charset = input.Request.Form["charset"];
                var totalFee = decimal.Parse(input.Request.Form["total_fee"]);
                var businessParams = input.Request.Form["business_params"];
                if (string.IsNullOrWhiteSpace(businessParams))
                {
                    throw new BusinessException("自定义参数不允许为空！");
                }

                return await Task.FromResult(new ExecPayNotifyOutputDto()
                {
                    BusinessParams = businessParams,
                    OutTradeNo = outTradeNo,
                    TotalFee = totalFee,
                    TradeNo = tradeNo,
                    SuccessResult = "success"
                });
            }
        }
    }
}

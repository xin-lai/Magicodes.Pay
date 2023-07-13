using Magicodes.Pay.Icbc;
using Magicodes.Pay.Icbc.Builder;
using Magicodes.Pay.Notify.Models;
using Magicodes.Pay.Volo.Abp.Dto;
using Magicodes.Pay.Volo.Abp.Registers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Json;

namespace Magicodes.Pay.Volo.Abp.Icbcpay
{
    /// <summary>
    /// 支付配置
    /// </summary>
    public class IcbcpayPaymentRegister : PaymentRegisterBase
    {
        public IcbcpayPaymentRegister(IServiceProvider serviceProvider, IJsonSerializer jsonSerializer, ILogger<PaymentRegisterBase> logger) : base(serviceProvider, jsonSerializer, logger)
        {
        }

        /// <summary>
        /// 关键字
        /// </summary>
        public override string Key { get; set; } = "Icbcpay";

        public override void Build(Action<string, string> logAction)
        {
            IcbcpayBuilder.Create()
                .WithLoggerAction(logAction)
                .RegisterGetPayConfigFunc(() => GetConfigFromConfigOrSettingsByKey<IcbcpaySettings>().Result).Build();
        }

        public override async Task<ExecPayNotifyOutputDto> ExecPayNotifyAsync(PayNotifyInput input)
        {
            {
                var api = serviceProvider.GetRequiredService<IIcbcpayAppService>();
                var dictionary = input.Request.Form.ToDictionary(p => p.Key,
                    p2 => p2.Value.FirstOrDefault()?.ToString());

                var result = api.PayNotifyHandler(dictionary); 

                return await Task.FromResult(new ExecPayNotifyOutputDto()
                {
                    BusinessParams = result.BusinessParams,
                    OutTradeNo = result.OutTradeNo,
                    TradeNo = result.TradeNo,
                    TotalFee = result.TotalFee,
                    SuccessResult = result.SuccessResult
                });

            }
        }
    }
}

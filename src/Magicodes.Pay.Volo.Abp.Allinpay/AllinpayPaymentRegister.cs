using System;
using System.Linq;
using System.Threading.Tasks;
using Magicodes.Pay.Allinpay;
using Magicodes.Pay.Allinpay.Builder;
using Magicodes.Pay.Notify.Models;
using Magicodes.Pay.Volo.Abp.Dto;
using Magicodes.Pay.Volo.Abp.Registers;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp;
using Volo.Abp.Json;
using Volo.Abp.Settings;

namespace Magicodes.Pay.Volo.Abp.Allinpay
{
    /// <summary>
    /// 通联支付支付配置
    /// </summary>
    public class AllinpayPaymentRegister : PaymentRegisterBase
    {
        public AllinpayPaymentRegister(IServiceProvider serviceProvider, IJsonSerializer jsonSerializer, ISettingProvider settingProvider) : base(serviceProvider, jsonSerializer, settingProvider)
        {
        }

        /// <summary>
        /// 关键字
        /// </summary>
        public override string Key { get; set; } = "Allinpay";

        public override void Build(Action<string, string> logAction)
        {
            AllinpayBuilder.Create()
                .WithLoggerAction(logAction)
                .RegisterGetPayConfigFunc(() => GetConfigFromConfigOrSettingsByKey<AllinpaySettings>().Result).Build();
        }

        public override async Task<ExecPayNotifyOutputDto> ExecPayNotifyAsync(PayNotifyInput input)
        {
            {
                var api = serviceProvider.GetRequiredService<IAllinpayAppService>();
                var dictionary = input.Request.Form.ToDictionary(p => p.Key,
                    p2 => p2.Value.FirstOrDefault()?.ToString());
                //签名校验
                if (!api.PayNotifyHandler(dictionary))
                {
                    throw new BusinessException("通联支付签名错误！");
                }

                // ReSharper disable once StringLiteralTypo
                var outTradeNo = input.Request.Form["outtrxid"];
                var tradeNo = input.Request.Form["trxid"];
                var totalFee = decimal.Parse(input.Request.Form["trxamt"]) / 100;
                // ReSharper disable once IdentifierTypo
                var trxreserved = input.Request.Form["trxreserved"];
                if (string.IsNullOrWhiteSpace(trxreserved))
                {
                    throw new BusinessException("自定义参数不允许为空！");
                }

                return await Task.FromResult(new ExecPayNotifyOutputDto()
                {
                    BusinessParams = trxreserved,
                    OutTradeNo = outTradeNo,
                    TradeNo = tradeNo,
                    TotalFee = totalFee,
                    SuccessResult = "success"
                });

            }
        }
    }
}

using Magicodes.Pay.Notify.Models;
using Magicodes.Pay.Volo.Abp.Dto;
using Magicodes.Pay.Volo.Abp.Registers;
using Magicodes.Pay.Wxpay.Builder;
using Magicodes.Pay.Wxpay.Config;
using Magicodes.Pay.Wxpay.Pay;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Json;
using Volo.Abp.Settings;

namespace Magicodes.Pay.Volo.Abp.Wxpay
{
    /// <summary>
    /// 微信支付配置
    /// </summary>
    public class WxpayPaymentRegister : PaymentRegisterBase
    {
        public WxpayPaymentRegister(IServiceProvider serviceProvider, IJsonSerializer jsonSerializer, ILogger<PaymentRegisterBase> logger) : base(serviceProvider, jsonSerializer, logger)
        {
        }

        /// <summary>
        /// 关键字
        /// </summary>
        public override string Key { get; set; } = "Wxpay";

        public override void Build(Action<string, string> logAction)
        {
            WeChatPayBuilder.Create()
                .WithLoggerAction(logAction)
                .RegisterGetPayConfigFunc(() => GetConfigFromConfigOrSettingsByKey<DefaultWeChatPayConfig>().Result).Build();
        }

        public override async Task<ExecPayNotifyOutputDto> ExecPayNotifyAsync(PayNotifyInput input)
        {
            var api = serviceProvider.GetRequiredService<WeChatPayApi>();
            {
                var outDto = new ExecPayNotifyOutputDto();
                var result = await api.PayNotifyHandler(input.Request.Body, (output, error) =>
                {
                    //获取微信支付自定义数据
                    if (string.IsNullOrWhiteSpace(output.Attach))
                    {
                        throw new BusinessException("自定义参数不允许为空！");
                    }
                    var outTradeNo = output.OutTradeNo;
                    var totalFee = decimal.Parse(output.TotalFee) / 100;
                    outDto.TradeNo = output.TransactionId;
                    outDto.BusinessParams = output.Attach;
                    outDto.TotalFee = totalFee;
                    outDto.OutTradeNo = outTradeNo;
                });
                outDto.SuccessResult = result;
                return outDto;
            }
        }
    }
}

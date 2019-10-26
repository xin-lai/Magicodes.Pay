using System;
using System.Linq;
using System.Threading.Tasks;
using Abp.Dependency;
using Abp.UI;
using Magicodes.Pay.Abp.Dto;
using Magicodes.Pay.Abp.Registers;
using Magicodes.Pay.Allinpay;
using Magicodes.Pay.Allinpay.Builder;
using Magicodes.Pay.Notify.Models;

namespace Magicodes.Pay.Abp.Allinpay
{
    /// <summary>
    /// 通联支付支付配置
    /// </summary>
    public class AllinpayPaymentRegister : PaymentRegisterBase
    {
        /// <summary>
        /// 关键字
        /// </summary>
        public override string Key { get; set; } = "Allinpay";

        public override async Task Build(Action<string, string> logAction)
        {
            //注册支付API
            if (IocManager.IsRegistered<IAllinpayAppService>()) return;

            AllinpayBuilder.Create()
                .WithLoggerAction(logAction)
                .RegisterGetPayConfigFunc(() => GetConfigFromConfigOrSettingsByKey<AllinpaySettings>().Result).Build();

            IocManager.Register<IAllinpayAppService, AllinpayAppService>(DependencyLifeStyle.Transient);
            await Task.FromResult(0);
        }

        public override async Task<ExecPayNotifyOutputDto> ExecPayNotifyAsync(PayNotifyInput input)
        {
            using (var obj = IocManager.ResolveAsDisposable<IAllinpayAppService>())
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
                var totalFee = (decimal.Parse(input.Request.Form["trxamt"]) / 100);
                // ReSharper disable once IdentifierTypo
                var trxreserved = input.Request.Form["trxreserved"];
                if (string.IsNullOrWhiteSpace(trxreserved))
                {
                    throw new UserFriendlyException("自定义参数不允许为空！");
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

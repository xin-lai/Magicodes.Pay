using Abp.Dependency;
using Magicodes.Pay.Abp.Dto;
using Magicodes.Pay.Abp.Registers;
using Magicodes.Pay.Icbc;
using Magicodes.Pay.Icbc.Builder;
using Magicodes.Pay.Notify.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Magicodes.Pay.Abp.Icbcpay
{
    /// <summary>
    /// 工行支付支付配置
    /// </summary>
    public class IcbcpayPaymentRegister : PaymentRegisterBase
    {
        /// <summary>
        /// 关键字
        /// </summary>
        public override string Key { get; set; } = "Icbcpay";

        public override async Task Build(Action<string, string> logAction)
        {
            //注册支付API
            if (IocManager.IsRegistered<IIcbcpayAppService>()) return;

            IcbcpayBuilder.Create()
                .WithLoggerAction(logAction)
                .RegisterGetPayConfigFunc(() => GetConfigFromConfigOrSettingsByKey<IcbcpaySettings>().Result).Build();

            IocManager.Register<IIcbcpayAppService, IcbcpayAppService>(DependencyLifeStyle.Transient);
            await Task.FromResult(0);
        }

        public override async Task<ExecPayNotifyOutputDto> ExecPayNotifyAsync(PayNotifyInput input)
        {
            using (var obj = IocManager.ResolveAsDisposable<IIcbcpayAppService>())
            {
                var api = obj.Object;
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

using System;
using System.Linq;
using System.Threading.Tasks;
using Abp.Dependency;
using Abp.UI;
using Magicodes.Pay.Abp.Dto;
using Magicodes.Pay.Abp.Registers;
using Magicodes.Pay.Alipay;
using Magicodes.Pay.Alipay.Builder;
using Magicodes.Pay.Notify.Models;

namespace Magicodes.Pay.Abp.Alipay
{
    /// <summary>
    /// 支付宝支付配置
    /// </summary>
    public class AlipayPaymentRegister : PaymentRegisterBase
    {
        /// <summary>
        /// 关键字
        /// </summary>
        public override string Key { get; set; } = "Alipay";

        public override async Task Build(Action<string, string> logAction)
        {
            //注册支付API
            if (IocManager.IsRegistered<IAlipayAppService>()) return;

            AlipayBuilder.Create()
                .WithLoggerAction(logAction)
                .RegisterGetPayConfigFunc(() => GetConfigFromConfigOrSettingsByKey<AlipaySettings>().Result).Build();

            IocManager.Register<IAlipayAppService, AlipayAppService>(DependencyLifeStyle.Transient);
            await Task.FromResult(0);
        }


        public override async Task<ExecPayNotifyOutputDto> ExecPayNotifyAsync(PayNotifyInput input)
        {
            using (var obj = IocManager.ResolveAsDisposable<IAlipayAppService>())
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

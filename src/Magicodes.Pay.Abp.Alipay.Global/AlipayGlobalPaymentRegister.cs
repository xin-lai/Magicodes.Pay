using System;
using System.Linq;
using System.Threading.Tasks;
using Abp.Dependency;
using Abp.UI;
using Magicodes.Pay.Abp.Dto;
using Magicodes.Pay.Abp.Registers;
using Magicodes.Pay.Alipay.Global;
using Magicodes.Pay.Alipay.Global.Builder;
using Magicodes.Pay.Notify.Models;

namespace Magicodes.Pay.Abp.Alipay.Global
{
    /// <summary>
    /// 通联支付支付配置
    /// </summary>
    public class AlipayGlobalPaymentRegister : PaymentRegisterBase
    {
        /// <summary>
        /// 关键字
        /// </summary>
        public override string Key { get; set; } = "Global.alipay";

        public override async Task Build(Action<string, string> logAction)
        {
            //注册支付API
            if (IocManager.IsRegistered<IGlobalAlipayAppService>()) return;

            GlobalAlipayBuilder.Create()
                .WithLoggerAction(logAction)
                .RegisterGetPayConfigFunc(() => GetConfigFromConfigOrSettingsByKey<GlobalAlipaySettings>().Result).Build();

            IocManager.Register<IGlobalAlipayAppService, GlobalAlipayAppService>(DependencyLifeStyle.Transient);
            await Task.FromResult(0);
        }

        public override async Task<ExecPayNotifyOutputDto> ExecPayNotifyAsync(PayNotifyInput input)
        {
            using (var obj = IocManager.ResolveAsDisposable<IGlobalAlipayAppService>())
            {
                var api = obj.Object;

                var dictionary = input.Request.Form.ToDictionary(p => p.Key, p2 => p2.Value.FirstOrDefault()?.ToString());
                //签名校验
                if (!api.PayNotifyHandler(dictionary))
                {
                    throw new UserFriendlyException("支付宝支付签名错误！");
                }
                var outTradeNo = input.Request.Form["out_trade_no"];
                var tradeNo = input.Request.Form["trade_no"];
                var charset = input.Request.Form["charset"];
                var totalFee = Convert.ToDecimal(input.Request.Form["total_fee"]);
                //交易状态
                string tradeStatus = input.Request.Form["trade_status"];

                return await Task.FromResult(new ExecPayNotifyOutputDto()
                {
                    //待处理
                    BusinessParams = null,
                    OutTradeNo = outTradeNo,
                    TradeNo = tradeNo,
                    TotalFee = totalFee,
                    SuccessResult = "success"
                });

            }
        }
    }
}

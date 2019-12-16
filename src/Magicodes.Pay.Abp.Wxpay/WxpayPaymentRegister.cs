using System;
using System.Threading.Tasks;
using Abp.Dependency;
using Abp.UI;
using Magicodes.Pay.Abp.Dto;
using Magicodes.Pay.Abp.Registers;
using Magicodes.Pay.Notify.Models;
using Magicodes.Pay.Wxpay.Builder;
using Magicodes.Pay.Wxpay.Config;
using Magicodes.Pay.Wxpay.Pay;

namespace Magicodes.Pay.Abp.Wxpay
{
    /// <summary>
    /// 通联支付支付配置
    /// </summary>
    public class WxpayPaymentRegister : PaymentRegisterBase
    {
        /// <summary>
        /// 关键字
        /// </summary>
        public override string Key { get; set; } = "Wxpay";

        public override async Task Build(Action<string, string> logAction)
        {
            //注册支付API
            if (IocManager.IsRegistered<WeChatPayApi>()) return;

            WeChatPayBuilder.Create()
                .WithLoggerAction(logAction)
                .RegisterGetPayConfigFunc(() => GetConfigFromConfigOrSettingsByKey<DefaultWeChatPayConfig>().Result).Build();

            IocManager.Register<WeChatPayApi>(DependencyLifeStyle.Transient);
            await Task.FromResult(0);
        }

        public override async Task<ExecPayNotifyOutputDto> ExecPayNotifyAsync(PayNotifyInput input)
        {
            using (var obj = IocManager.ResolveAsDisposable<WeChatPayApi>())
            {
                var api = obj.Object;
                var outDto = new ExecPayNotifyOutputDto();
                var result = await api.PayNotifyHandler(input.Request.Body, async (output, error) =>
                {
                    //获取微信支付自定义数据
                    if (string.IsNullOrWhiteSpace(output.Attach))
                    {
                        throw new UserFriendlyException("自定义参数不允许为空！");
                    }

                    var outTradeNo = output.OutTradeNo;
                    var totalFee = int.Parse(output.TotalFee) / 100;
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


using Magicodes.Pay.Volo.Abp.Services;
using Magicodes.Pay.Wxpay.Pay;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Modularity;

namespace Magicodes.Pay.Volo.Abp.Wxpay
{
    [DependsOn(typeof(PayModule))]
    public class AbpWxpayModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            context.Services.AddTransient<WeChatPayApi>();

            context.Services.AddTransient<IToPayService, WeChatAppPayTopayService>();
            context.Services.AddTransient<IToPayService, WeChatMiniprogramTopayService>();
        }
    }
}

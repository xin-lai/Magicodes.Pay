using Magicodes.Pay.Alipay;
using Magicodes.Pay.Volo.Abp;
using Magicodes.Pay.Volo.Abp.Services;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Modularity;

namespace Magicodes.Pay.Abp.Alipay
{
    [DependsOn(typeof(PayModule))]
    public class AbpAlipayModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            context.Services.AddTransient<IAlipayAppService, AlipayAppService>();

            context.Services.AddTransient<IToPayService, AlipayAppPayTopayService>();
        }
    }
}

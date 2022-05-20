using Magicodes.Pay.Allinpay;
using Magicodes.Pay.Volo.Abp.Registers;
using Magicodes.Pay.Volo.Abp.Services;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Modularity;

namespace Magicodes.Pay.Volo.Abp.Allinpay
{
    [DependsOn(typeof(PayModule))]
    public class AbpAllinpayModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            context.Services.AddTransient<IAllinpayAppService, AllinpayAppService>();

            context.Services.AddTransient<IToPayService, AllinpayMiniprogramTopayService>();
            context.Services.AddTransient<IToPayService, AllinpayJsApiTopayService>();
        }
    }
}

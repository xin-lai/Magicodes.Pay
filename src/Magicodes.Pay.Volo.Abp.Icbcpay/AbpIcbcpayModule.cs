using Magicodes.Pay.Icbc;
using Magicodes.Pay.Volo.Abp.Registers;
using Magicodes.Pay.Volo.Abp.Services;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Modularity;

namespace Magicodes.Pay.Volo.Abp.Icbcpay
{
    [DependsOn(typeof(PayModule))]
    public class AbpIcbcpayModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            context.Services.AddTransient<IIcbcpayAppService, IcbcpayAppService>();
            context.Services.AddTransient<IToPayService, IcbcpayTopayService>();

        }
    }
}

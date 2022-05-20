using Magicodes.Pay.Notify;
using Magicodes.Pay.Notify.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;
using Volo.Abp;
using Volo.Abp.Json;
using Volo.Abp.Modularity;
using Volo.Abp.Timing;

namespace Magicodes.Pay.Volo.Abp
{
    [DependsOn(
        typeof(AbpJsonModule)
    )]
    public class PayModule : AbpModule
    {
        public override void PreConfigureServices(ServiceConfigurationContext context)
        {
            PreConfigure<AbpJsonOptions>((opt) =>
            {
                opt.UseHybridSerializer = false;
            });
        }

        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            context.Services.AddTransient<PayNotifyController>();

            Configure<AbpClockOptions>(options =>
            {
                options.Kind = DateTimeKind.Local;
            });

            context.Services.AddSingleton<IPaymentManager, PaymentManager>();
        }

        public override void OnApplicationInitialization(ApplicationInitializationContext context)
        {
            context.ServiceProvider.GetRequiredService<IPaymentManager>().Initialize();
        }

    }
}
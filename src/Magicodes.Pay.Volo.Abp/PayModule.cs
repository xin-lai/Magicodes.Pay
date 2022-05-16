﻿using Magicodes.Pay.Notify;
using Magicodes.Pay.Notify.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;
using Volo.Abp;
using Volo.Abp.Modularity;
using Volo.Abp.Timing;

namespace Magicodes.Pay.Volo.Abp
{
    //[DependsOn(
    //    typeof(AbpSettingManagementApplicationModule)
    //)]
    public class PayModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            context.Services.AddTransient<PayNotifyController>();

            Configure<AbpClockOptions>(options =>
            {
                options.Kind = DateTimeKind.Local;
            });
            
        }

        public override void OnApplicationInitialization(ApplicationInitializationContext context)
        {
            context.ServiceProvider.GetRequiredService<IPaymentManager>().Initialize();
        }

    }
}
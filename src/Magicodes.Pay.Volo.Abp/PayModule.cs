using Magicodes.Pay.Notify;
using Magicodes.Pay.Notify.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;
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


            ////支付回调设置
            //PayNotifyBuilder
            //    .Create()
            //    //设置日志记录
            //    .WithLoggerAction(logAction).WithPayNotifyFunc(async input => await ExecPayNotifyAsync(input)).Build();
        }

        //public override void OnApplicationInitialization(ApplicationInitializationContext context)
        //{
        //    var myService = context.ServiceProvider.GetService<MyService>();
        //    myService.DoSomething();
        //}
    }
}
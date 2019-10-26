using System.IO;
using Abp.EntityFrameworkCore;
using Abp.Modules;
using Abp.Reflection.Extensions;
using Abp.TestBase;
using Castle.MicroKernel.Registration;
using Magicodes.Pay.Tests.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Castle.Core;
using Castle.Core.Internal;
using Magicodes.Pay.Abp;
using Magicodes.Pay.Abp.Callbacks;
using Magicodes.Pay.Abp.Configs;


namespace Magicodes.Pay.Tests
{
    [DependsOn(
       typeof(AbpTestBaseModule),
       typeof(AbpEntityFrameworkCoreModule),
       typeof(PayModule)
       )]
    public class TestModule : AbpModule
    {
        public override void PreInitialize()
        {
            IocManager.IocContainer.Register(
                Component.For<IConfiguration>().Instance(GetConfiguration()).LifestyleSingleton()
                );

            IocManager.Register<IConfiguration>();
        }

        public override void Initialize()
        {
            ServiceCollectionRegistrar.Register(IocManager);
            IocManager.RegisterAssemblyByConvention(typeof(TestModule).GetAssembly());

            //注册自定义支付回调逻辑
            IocManager.IocContainer.Register(
                Classes.FromAssembly(typeof(TestModule).GetAssembly())
                    .BasedOn<IPaymentCallbackAction>()
                    .LifestyleTransient()
                    .Configure(component => component.Named(component.Implementation.FullName))
                    .WithServiceFromInterface()
                );

            //注册自定义支付配置逻辑
            IocManager.IocContainer.Register(
                Classes.FromAssembly(typeof(TestModule).GetAssembly())
                    .BasedOn<IPaymentConfigAction>()
                    .LifestyleTransient()
                    .Configure(component => component.Named(component.Implementation.FullName))
                    .WithServiceFromInterface()
            );
        }

        private static IConfiguration GetConfiguration()
        {
            var builder = new ConfigurationBuilder()
               .SetBasePath(Directory.GetCurrentDirectory())
               .AddJsonFile("appsettings.json", true, true);

            return builder.Build();
        }
    }
}
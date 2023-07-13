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
using Magicodes.Pay.Abp.Allinpay;
using Magicodes.Pay.Abp.Callbacks;
using Magicodes.Pay.Abp.Registers;
using Magicodes.Pay.Abp.Icbcpay;

namespace Magicodes.Pay.Tests
{
    [DependsOn(
       typeof(AbpTestBaseModule),
       typeof(AbpEntityFrameworkCoreModule),
       typeof(AbpAllinpayModule),
        typeof(AbpIcbcpayModule)
       )]
    public class TestModule : AbpModule
    {
        public override void PreInitialize()
        {
            IocManager.IocContainer.Register(
                Component.For<IConfiguration>().Instance(GetConfiguration()).LifestyleSingleton()
                );

            //IocManager.Register<IConfiguration>();
        }

        public override void Initialize()
        {
            ServiceCollectionRegistrar.Register(IocManager);
            IocManager.RegisterAssemblyByConvention(typeof(TestModule).GetAssembly());


            IocManager.IocContainer.Register(
                ////注册自定义支付回调逻辑
                //Classes.FromAssembly(typeof(TestModule).GetAssembly())
                //    .BasedOn<IPaymentCallbackAction>()
                //    .LifestyleTransient()
                //    .Configure(component => component.Named(component.Implementation.FullName))
                //    .WithServiceFromInterface(),
                //注册自定义支付回调逻辑
                Classes.FromAssembly(typeof(TestModule).GetAssembly())
                    .BasedOn<IPaymentCallbackAction>()
                    .LifestyleTransient()
                    .Configure(component => component.Named(component.Implementation.FullName))
                    .WithServiceFromInterface()
                    //,
                ////注册自定义支付配置逻辑
                //Classes.FromAssembly(typeof(TestModule).GetAssembly())
                //    .BasedOn<IPaymentRegister>()
                //    .LifestyleTransient()
                //    .Configure(component => component.Named(component.Implementation.FullName))
                //    .WithServiceFromInterface()
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
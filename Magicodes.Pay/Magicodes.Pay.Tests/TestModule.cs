using System.IO;
using Abp.EntityFrameworkCore;
using Abp.Modules;
using Abp.Reflection.Extensions;
using Abp.TestBase;
using Castle.MicroKernel.Registration;
using Magicodes.Pay.Tests.DependencyInjection;
using Microsoft.Extensions.Configuration;

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
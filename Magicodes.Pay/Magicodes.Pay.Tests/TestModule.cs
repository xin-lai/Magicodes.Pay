using Abp.Modules;
using Abp.TestBase;
using Microsoft.Extensions.Configuration;
using System.IO;
using Castle.MicroKernel.Registration;

namespace Magicodes.Admin.Tests.Custom.App_Payment
{
    [DependsOn(
       typeof(AbpTestBaseModule))]
    public class TestModule : AbpModule
    {
        public override void PreInitialize()
        {
            IocManager.IocContainer.Register(
                Component.For<IConfiguration>().Instance(GetConfiguration()).LifestyleSingleton()
                );

            IocManager.Register<IConfiguration>();
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
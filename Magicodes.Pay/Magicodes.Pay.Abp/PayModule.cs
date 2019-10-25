using System.IO;
using Abp.Modules;
using Abp.Reflection.Extensions;
using Castle.MicroKernel.Registration;
using Microsoft.Extensions.Configuration;

namespace Magicodes.Pay.Tests
{
    public class PayModule : AbpModule
    {
        public override void PreInitialize()
        {
            
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(PayModule).GetAssembly());
        }

        public override void PostInitialize()
        {
        }

    }
}
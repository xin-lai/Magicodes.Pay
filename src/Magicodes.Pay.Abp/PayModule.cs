using Abp.Modules;
using Abp.Reflection.Extensions;

namespace Magicodes.Pay.Abp
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
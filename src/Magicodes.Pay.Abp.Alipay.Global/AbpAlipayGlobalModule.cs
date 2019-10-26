using Abp.Modules;
using Abp.Reflection.Extensions;
using Castle.MicroKernel.Registration;
using Magicodes.Pay.Abp.Registers;

namespace Magicodes.Pay.Abp.Allinpay
{
    public class AbpAlipayGlobalModule : AbpModule
    {
        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(AbpAlipayGlobalModule).GetAssembly());

            //注册自定义支付配置逻辑
            IocManager.IocContainer.Register(
                Classes.FromAssembly(typeof(AbpAlipayGlobalModule).GetAssembly())
                    .BasedOn<IPaymentRegister>()
                    .LifestyleTransient()
                    .Configure(component => component.Named(component.Implementation.FullName))
                    .WithServiceFromInterface()
            );
        }
    }
}

using Abp.Modules;
using Abp.Reflection.Extensions;
using Castle.MicroKernel.Registration;
using Magicodes.Pay.Abp.Registers;

namespace Magicodes.Pay.Abp.Allinpay
{
    public class AbpWxpayModule : AbpModule
    {
        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(AbpWxpayModule).GetAssembly());

            //注册自定义支付配置逻辑
            IocManager.IocContainer.Register(
                Classes.FromAssembly(typeof(AbpWxpayModule).GetAssembly())
                    .BasedOn<IPaymentRegister>()
                    .LifestyleTransient()
                    .Configure(component => component.Named(component.Implementation.FullName))
                    .WithServiceFromInterface()
            );
        }
    }
}

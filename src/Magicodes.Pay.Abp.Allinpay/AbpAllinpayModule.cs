using Abp.Modules;
using Abp.Reflection.Extensions;
using Castle.MicroKernel.Registration;
using Magicodes.Pay.Abp.Registers;

namespace Magicodes.Pay.Abp.Allinpay
{
    public class AbpAllinpayModule : AbpModule
    {
        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(AbpAllinpayModule).GetAssembly());

            ////注册自定义支付回调逻辑
            //IocManager.IocContainer.Register(
            //    Classes.FromAssembly(typeof(TestModule).GetAssembly())
            //        .BasedOn<IPaymentCallbackAction>()
            //        .LifestyleTransient()
            //        .Configure(component => component.Named(component.Implementation.FullName))
            //        .WithServiceFromInterface()
            //);

            //注册自定义支付配置逻辑
            IocManager.IocContainer.Register(
                Classes.FromAssembly(typeof(AbpAllinpayModule).GetAssembly())
                    .BasedOn<IPaymentRegister>()
                    .LifestyleTransient()
                    .Configure(component => component.Named(component.Implementation.FullName))
                    .WithServiceFromInterface()
            );
        }
    }
}

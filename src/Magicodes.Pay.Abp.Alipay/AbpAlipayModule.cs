using System;
using Abp.Modules;
using Abp.Reflection.Extensions;
using Castle.MicroKernel.Registration;
using Magicodes.Pay.Abp.Callbacks;
using Magicodes.Pay.Abp.Registers;

namespace Magicodes.Pay.Abp.Alipay
{
    public class AbpAlipayModule : AbpModule
    {
        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(AbpAlipayModule).GetAssembly());

            //注册自定义支付配置逻辑
            IocManager.IocContainer.Register(
                Classes.FromAssembly(typeof(AbpAlipayModule).GetAssembly())
                    .BasedOn<IPaymentRegister>()
                    .LifestyleTransient()
                    .Configure(component => component.Named(component.Implementation.FullName))
                    .WithServiceFromInterface()
            );
        }
    }
}

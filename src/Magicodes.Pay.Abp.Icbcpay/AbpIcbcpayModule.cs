﻿using Abp.Modules;
using Abp.Reflection.Extensions;
using Castle.MicroKernel.Registration;
using Magicodes.Pay.Abp.Registers;
using Magicodes.Pay.Abp.Services;

namespace Magicodes.Pay.Abp.Icbcpay
{
    [DependsOn(typeof(PayModule))]
    public class AbpIcbcpayModule : AbpModule
    {
        public override void Initialize()
        {
            var assembly = typeof(AbpIcbcpayModule).GetAssembly();

            IocManager.RegisterAssemblyByConvention(assembly);

            
            IocManager.IocContainer.Register(
                //注册自定义支付配置逻辑
                Classes.FromAssembly(assembly)
                    .BasedOn<IPaymentRegister>()
                    .LifestyleTransient()
                    .Configure(component => component.Named(component.Implementation.FullName))
                    .WithServiceFromInterface(),
                //注册支付服务
                Classes.FromAssembly(assembly)
                    .BasedOn<IToPayService>()
                    .LifestyleTransient()
                    .Configure(component => component.Named(component.Implementation.FullName))
                    .WithServiceFromInterface()
            );

        }
    }
}

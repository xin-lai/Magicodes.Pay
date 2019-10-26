using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Abp.Configuration;
using Abp.Dependency;
using Castle.Core.Logging;
using Magicodes.Pay.Abp;
using Microsoft.Extensions.Configuration;
using Abp.Json;
using Magicodes.Pay.Abp.Configs;
using Magicodes.Pay.Alipay;
using Magicodes.Pay.Alipay.Builder;

namespace Magicodes.Pay.Tests.Configs
{
    /// <summary>
    /// 支付宝支付配置
    /// </summary>
    public class AlipayPaymentConfigAction : PaymentConfigActionBase
    {
        /// <summary>
        /// 关键字
        /// </summary>
        public override string Key { get; set; } = "Alipay";

        public override async Task Build(Action<string, string> logAction)
        {
            //注册支付API
            if (IocManager.IsRegistered<IAlipayAppService>()) return;

            AlipayBuilder.Create()
                .WithLoggerAction(logAction)
                .RegisterGetPayConfigFunc(() => GetConfigFromConfigOrSettingsByKey<AlipaySettings>().Result).Build();

            IocManager.Register<IAlipayAppService, AlipayAppService>(DependencyLifeStyle.Transient);
            await Task.FromResult(0);
        }

    }
}

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
using Magicodes.Allinpay;
using Magicodes.Allinpay.Builder;
using Magicodes.Pay.Abp.Configs;

namespace Magicodes.Pay.Tests.Configs
{
    /// <summary>
    /// 支付宝支付配置
    /// </summary>
    public class AllinpayPaymentConfigAction : PaymentConfigActionBase
    {
        /// <summary>
        /// 关键字
        /// </summary>
        public override string Key { get; set; } = "Allinpay";

        public override async Task Build(Action<string, string> logAction)
        {
            //注册支付API
            if (IocManager.IsRegistered<IAllinpayAppService>()) return;

            //通联支付配置
            AllinpayBuilder.Create()
                //设置日志记录
                .WithLoggerAction(logAction)
                .RegisterGetPayConfigFunc(() => GetConfigFromConfigOrSettingsByKey<AllinpaySettings>().Result).Build();

            IocManager.Register<IAllinpayAppService, AllinpayAppService>(DependencyLifeStyle.Transient);

            await Task.FromResult(0);
        }

    }
}

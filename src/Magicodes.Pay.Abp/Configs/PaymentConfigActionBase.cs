using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Abp.Configuration;
using Abp.Dependency;
using Abp.Json;
using Microsoft.Extensions.Configuration;

namespace Magicodes.Pay.Abp.Configs
{
    /// <summary>
    /// 
    /// </summary>
    public class PaymentConfigActionBase : IPaymentConfigAction
    {
        public IIocManager IocManager { get; set; }
        public IConfiguration AppConfiguration { get; set; }

        /// <summary>
        /// 关键字
        /// </summary>
        public virtual string Key { get; set; }

        public virtual Task Build(Action<string, string> logAction)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 根据key从站点配置文件或设置中获取支付配置
        /// </summary>
        /// <typeparam name="TConfig"></typeparam>
        /// <returns></returns>
        public virtual Task<TConfig> GetConfigFromConfigOrSettingsByKey<TConfig>() where TConfig : class, new()
        {
            var settings = AppConfiguration.GetSection(key: Key)?.Get<TConfig>();
            if (settings != null) return Task.FromResult(settings);

            using (var obj = IocManager.ResolveAsDisposable<ISettingManager>())
            {
                var value = obj.Object.GetSettingValue(Key);
                if (string.IsNullOrWhiteSpace(value))
                {
                    return Task.FromResult<TConfig>(null);
                }
                settings = value.FromJsonString<TConfig>();
                return Task.FromResult(settings);
            }
        }
    }
}

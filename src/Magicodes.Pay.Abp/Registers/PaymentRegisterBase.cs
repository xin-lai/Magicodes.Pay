using System;
using System.Threading.Tasks;
using Abp.Configuration;
using Abp.Dependency;
using Abp.Json;
using Abp.UI;
using Magicodes.Pay.Abp.Callbacks;
using Magicodes.Pay.Abp.Dto;
using Magicodes.Pay.Notify.Models;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;

namespace Magicodes.Pay.Abp.Registers
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class PaymentRegisterBase : IPaymentRegister
    {
        /// <summary>
        /// 
        /// </summary>
        public IIocManager IocManager { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public IConfiguration AppConfiguration { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public IPaymentManager PaymentManager { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="outTradeNo"></param>
        /// <param name="transactionId"></param>
        /// <param name="totalFee"></param>
        /// <param name="customData"></param>
        /// <returns></returns>
        protected async Task PayActionAsync(string outTradeNo, string transactionId, int totalFee, string customData)
        {
            if (string.IsNullOrWhiteSpace(customData))
            {
                throw new UserFriendlyException("请配置自定义参数！");
            }
            //using (var uow = _unitOfWorkManager.Begin())
            {
                //目前仅用支付参数的业务字段存储key，自定义数据在交易日志的CustomData中
                var key = customData.Contains("{") ? customData.FromJsonString<JObject>()["key"]?.ToString() : customData;
                await PaymentManager.ExecuteCallback(key, outTradeNo, transactionId, totalFee);
            }
        }

        /// <summary>
        /// 关键字
        /// </summary>
        public virtual string Key { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="logAction"></param>
        /// <returns></returns>
        public virtual Task Build(Action<string, string> logAction)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 支付回调
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public virtual Task<ExecPayNotifyOutputDto> ExecPayNotifyAsync(PayNotifyInput input)
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
            var settings = AppConfiguration?.GetSection(key: Key)?.Get<TConfig>();
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

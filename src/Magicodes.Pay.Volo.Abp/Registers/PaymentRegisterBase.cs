using System;
using System.Threading.Tasks;
using Magicodes.Pay.Notify.Models;
using Magicodes.Pay.Volo.Abp.Dto;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using Volo.Abp;
using Volo.Abp.Json;
using Volo.Abp.Settings;

namespace Magicodes.Pay.Volo.Abp.Registers
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class PaymentRegisterBase : IPaymentRegister
    {

        /// <summary>
        /// 
        /// </summary>
        public IConfiguration AppConfiguration { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public IPaymentManager PaymentManager { get; set; }

        private readonly ISettingProvider settingProvider;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="outTradeNo"></param>
        /// <param name="transactionId"></param>
        /// <param name="totalFee"></param>
        /// <param name="customData"></param>
        /// <returns></returns>
        protected async Task PayActionAsync(string outTradeNo,
                                            string transactionId,
                                            int totalFee,
                                            string customData,
                                            IServiceProvider serviceProvider,
                                            IJsonSerializer jsonSerializer,
                                            ISettingProvider settingProvider)
        {
            settingProvider = settingProvider;
            if (string.IsNullOrWhiteSpace(customData))
            {
                throw new BusinessException("请配置自定义参数！");
            }
            {
                var jobj= jsonSerializer.Deserialize<JObject>(customData);
                //目前仅用支付参数的业务字段存储key，自定义数据在交易日志的CustomData中
                var key = customData.Contains("{") ? jobj["key"]?.ToString() : customData;
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
        public virtual Task<ExecPayNotifyOutputDto> ExecPayNotifyAsync(PayNotifyWithGuidInput input)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 根据key从站点配置文件或设置中获取支付配置
        /// </summary>
        /// <typeparam name="TConfig"></typeparam>
        /// <returns></returns>
        public virtual async Task<TConfig> GetConfigFromConfigOrSettingsByKey<TConfig>() where TConfig : class, new()
        {
            var settings = AppConfiguration?.GetSection(key: Key)?.Get<TConfig>();
            if (settings != null) return await Task.FromResult(settings);

            {
                var value = await settingProvider.GetAsync<TConfig>(Key);
                if (string.IsNullOrWhiteSpace(value))
                {
                    return await Task.FromResult<TConfig>(null);
                }
                settings = value.FromJsonString<TConfig>();
                return await Task.FromResult(settings);
            }
        }
    }
}

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
        protected readonly ISettingProvider settingProvider;
        protected readonly IJsonSerializer jsonSerializer;
        protected readonly IServiceProvider serviceProvider;

        public PaymentRegisterBase(IServiceProvider serviceProvider,
                                   IJsonSerializer jsonSerializer,
                                   ISettingProvider settingProvider)
        {
            this.serviceProvider = serviceProvider;
            this.jsonSerializer = jsonSerializer;
            this.settingProvider = settingProvider;
        }

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
        protected async Task PayActionAsync(string outTradeNo,
                                            string transactionId,
                                            int totalFee,
                                            string customData
                                            )
        {

            if (string.IsNullOrWhiteSpace(customData))
            {
                throw new BusinessException("请配置自定义参数！");
            }
            {
                var jobj = jsonSerializer.Deserialize<JObject>(customData);
                //目前仅用支付参数的业务字段存储key，自定义数据在交易日志的CustomData中
                var key = customData.Contains("{") ? jobj["key"]?.ToString() : customData;
                if (key != null)
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
        public virtual void Build(Action<string, string> logAction)
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
        public virtual async Task<TConfig> GetConfigFromConfigOrSettingsByKey<TConfig>() where TConfig : class, new()
        {
            var settings = AppConfiguration?.GetSection(key: Key)?.Get<TConfig>();
            if (settings != null) return await Task.FromResult(settings);
            {
                var value = await settingProvider.GetOrNullAsync(Key);
                if (string.IsNullOrWhiteSpace(value))
                {
                    return await Task.FromResult<TConfig>(result: null);
                }
                settings = jsonSerializer.Deserialize<TConfig>(value);
                return await Task.FromResult(settings);
            }
        }
    }
}

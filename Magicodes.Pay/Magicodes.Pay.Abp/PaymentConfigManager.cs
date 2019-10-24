using System;
using Abp.Configuration;
using Abp.Dependency;
using Abp.Domain.Uow;
using Abp.Extensions;
using Castle.Core.Logging;
using Magicodes.Alipay;
using Magicodes.Alipay.Builder;
using Magicodes.Allinpay;
using Magicodes.Allinpay.Builder;
using Magicodes.Pay.WeChat;
using Magicodes.Pay.WeChat.Builder;
using Magicodes.Pay.WeChat.Config;
using Magicodes.PayNotify;
using Magicodes.PayNotify.Builder;
using Microsoft.Extensions.Configuration;

namespace Magicodes.Pay.Abp
{
    /// <summary>
    /// 支付配置管理器
    /// </summary>
    public class PaymentConfigManager : IPaymentConfigManager
    {
        private readonly ISettingManager _settingManager;
        private readonly IConfiguration _appConfiguration;
        private readonly IIocManager _iocManager;
        private readonly IUnitOfWorkManager _unitOfWorkManager;

        /// <summary>
        /// 
        /// </summary>
        public ILogger Logger { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="settingManager"></param>
        /// <param name="appConfiguration"></param>
        /// <param name="iocManager"></param>
        /// <param name="unitOfWorkManager"></param>
        public PaymentConfigManager(ISettingManager settingManager, IConfiguration appConfiguration, IIocManager iocManager, IUnitOfWorkManager unitOfWorkManager)
        {
            this._settingManager = settingManager;
            this._appConfiguration = appConfiguration;
            this._iocManager = iocManager;
            this._unitOfWorkManager = unitOfWorkManager;
            this.Logger = NullLogger.Instance;
        }



        /// <summary>
        ///   Implementors should perform any initialization logic.
        /// </summary>
        public void Initialize()
        {

            //日志函数
            void LogAction(string tag, string message)
            {
                if (tag.Equals("error", StringComparison.CurrentCultureIgnoreCase))
                {
                    Logger.Error(message);
                }
                else
                {
                    Logger.Debug(message);
                }
            }

            PayConfig(LogAction);
            PayNotifyConfig(LogAction);
        }

        /// <summary>
        /// 支付配置
        /// </summary>
        /// <param name="logAction"></param>
        public void PayConfig(Action<string, string> logAction)
        {
            #region 支付配置
            WeChatPayConfig(logAction);
            AlipayConfig(logAction);
            AllinpayConfig(logAction);
            #endregion
        }


        /// <summary>
        /// 支付宝支付配置
        /// </summary>
        /// <param name="logAction"></param>
        /// <returns></returns>
        private void AlipayConfig(Action<string, string> logAction)
        {
            //注册支付宝支付API
            if (!_iocManager.IsRegistered<IAlipayAppService>())
            {
                _iocManager.Register<IAlipayAppService, AlipayAppService>(DependencyLifeStyle.Transient);

                AlipayBuilder.Create()
                .WithLoggerAction(logAction)
                .RegisterGetPayConfigFunc(() =>
                {
                    var settingManager = _settingManager;
                    var config = _appConfiguration;
                    AlipaySettings alipaySettings = null;
                    //if (Convert.ToBoolean(settingManager.GetSettingValueAsync(AppSettings.AliPayManagement.IsActive).Result))
                    //{
                    //    alipaySettings = new AlipaySettings
                    //    {
                    //        AlipayPublicKey = settingManager.GetSettingValueAsync(AppSettings.AliPayManagement.AlipayPublicKey).Result,
                    //        AppId = settingManager.GetSettingValueAsync(AppSettings.AliPayManagement.AppId).Result,
                    //        Uid = settingManager.GetSettingValueAsync(AppSettings.AliPayManagement.Uid).Result,
                    //        PrivateKey = settingManager.GetSettingValueAsync(AppSettings.AliPayManagement.PrivateKey).Result,
                    //    };

                    //    var charSet = settingManager.GetSettingValueAsync(AppSettings.AliPayManagement.CharSet).Result;
                    //    if (!charSet.IsNullOrWhiteSpace())
                    //    {
                    //        alipaySettings.CharSet = charSet;
                    //    }
                    //    var gatewayurl = settingManager.GetSettingValueAsync(AppSettings.AliPayManagement.Gatewayurl).Result;
                    //    if (!gatewayurl.IsNullOrWhiteSpace())
                    //    {
                    //        alipaySettings.Gatewayurl = gatewayurl;
                    //    }
                    //    var notify = settingManager.GetSettingValueAsync(AppSettings.AliPayManagement.Notify).Result;
                    //    if (!notify.IsNullOrWhiteSpace())
                    //    {
                    //        alipaySettings.Notify = notify;
                    //    }
                    //    var signType = settingManager.GetSettingValueAsync(AppSettings.AliPayManagement.SignType).Result;
                    //    if (!signType.IsNullOrWhiteSpace())
                    //    {
                    //        alipaySettings.SignType = signType;
                    //    }
                    //}
                    //else 
                    if (!config["Alipay:IsEnabled"].IsNullOrWhiteSpace() &&
                        Convert.ToBoolean(config["Alipay:IsEnabled"]))
                    {
                        alipaySettings = new AlipaySettings
                        {
                            AlipayPublicKey = config["Alipay:PublicKey"],
                            AppId = config["Alipay:AppId"],
                            Uid = config["Alipay:Uid"],
                            PrivateKey = config["Alipay:PrivateKey"]
                        };
                        if (!config["Alipay:CharSet"].IsNullOrWhiteSpace())
                        {
                            alipaySettings.CharSet = config["Alipay:CharSet"];
                        }

                        if (!config["Alipay:Gatewayurl"].IsNullOrWhiteSpace())
                        {
                            alipaySettings.Gatewayurl = config["Alipay:Gatewayurl"];
                        }

                        if (!config["Alipay:Notify"].IsNullOrWhiteSpace())
                        {
                            alipaySettings.Notify = config["Alipay:Notify"];
                        }

                        if (!config["Alipay:SignType"].IsNullOrWhiteSpace())
                        {
                            alipaySettings.SignType = config["Alipay:SignType"];
                        }
                    }

                    return alipaySettings;
                }).Build();
            }
        }

        /// <summary>
        /// 通联支付配置
        /// </summary>
        /// <param name="logAction"></param>
        /// <returns></returns>
        private void AllinpayConfig(Action<string, string> logAction)
        {
            //注册支付API
            if (_iocManager.IsRegistered<IAllinpayAppService>()) return;

            //通联支付配置
            AllinpayBuilder.Create()
                //设置日志记录
                .WithLoggerAction(logAction)
                .RegisterGetPayConfigFunc(() =>
                {
                    var config = _appConfiguration;
                    AllinpaySettings allinpaySettings = null;
                    if (!config["Allinpay:IsEnabled"].IsNullOrWhiteSpace() && Convert.ToBoolean(config["Allinpay:IsEnabled"]))
                    {
                        allinpaySettings = new AllinpaySettings
                        {
                            AppId = config["Allinpay:AppId"],
                            CusId = config["Allinpay:CusId"],
                            AppKey = config["Allinpay:AppKey"],
                            ApiGateWay = config["Allinpay:ApiGateWay"],
                            Version = config["Allinpay:Version"],
                            NotifyUrl = config["Allinpay:NotifyUrl"],
                            WeChatAppId = config["Allinpay:WeChatAppId"]
                        };
                    }

                    return allinpaySettings;
                }).Build();

            _iocManager.Register<IAllinpayAppService, AllinpayAppService>(DependencyLifeStyle.Transient);
        }


        /// <summary>
        /// 微信支付配置
        /// </summary>
        /// <param name="logAction"></param>
        /// <returns></returns>
        private void WeChatPayConfig(Action<string, string> logAction)
        {
            //注册微信支付API
            if (_iocManager.IsRegistered<WeChatPayApi>()) return;

            //微信支付配置
            WeChatPayBuilder.Create()
                //设置日志记录
                .WithLoggerAction(logAction)
                .RegisterGetPayConfigFunc(() =>
                {
                    {
                        var settingManager = _settingManager;
                        var config = _appConfiguration;
                        DefaultWeChatPayConfig weChatPayConfig = null;

                        //if (Convert.ToBoolean(settingManager.GetSettingValueAsync(AppSettings.WeChatPayManagement.IsActive).Result))
                        //{
                        //    weChatPayConfig = new DefaultWeChatPayConfig()
                        //    {
                        //        PayAppId = settingManager.GetSettingValueAsync(AppSettings.WeChatPayManagement.AppId).Result,
                        //        MchId = settingManager.GetSettingValueAsync(AppSettings.WeChatPayManagement.MchId).Result,
                        //        PayNotifyUrl = settingManager.GetSettingValueAsync(AppSettings.WeChatPayManagement.PayNotifyUrl).Result,
                        //        TenPayKey = settingManager.GetSettingValueAsync(AppSettings.WeChatPayManagement.TenPayKey).Result,
                        //    };
                        //}
                        //else
                        if (!config["WeChat:Pay:IsEnabled"].IsNullOrWhiteSpace() && Convert.ToBoolean(config["WeChat:Pay:IsEnabled"]))
                        {
                            weChatPayConfig = new DefaultWeChatPayConfig
                            {
                                MchId = config["WeChat:Pay:MchId"],
                                PayNotifyUrl = config["WeChat:Pay:NotifyUrl"],
                                TenPayKey = config["WeChat:Pay:TenPayKey"],
                                PayAppId = config["WeChat:Pay:AppId"]
                            };
                        }

                        return weChatPayConfig;
                    }
                }).Build();

            _iocManager.Register<WeChatPayApi>(DependencyLifeStyle.Transient);
        }


        /// <summary>
        /// 支付回调配置
        /// </summary>
        /// <param name="logAction"></param>
        public void PayNotifyConfig(Action<string, string> logAction)
        {
            if (_iocManager.IsRegistered<PayNotifyController>()) return;

            //注册支付回调控制器
            _iocManager.Register<PayNotifyController>(DependencyLifeStyle.Transient);

            //IPayNotifyManager payNotifyManager
            //支付回调设置
            PayNotifyBuilder
                .Create()
                //设置日志记录
                .WithLoggerAction(logAction).WithPayNotifyFunc(async input =>
                    {
                        using (var obj = _iocManager.ResolveAsDisposable<IPayNotifyManager>())
                        {
                            var payNotifyManager = obj.Object;
                            return await payNotifyManager.ExecPayNotifyAsync(input);
                        }
                    }
                ).Build();
        }
    }
}

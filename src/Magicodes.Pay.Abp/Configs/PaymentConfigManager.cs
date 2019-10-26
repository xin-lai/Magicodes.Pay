using System;
using System.Collections.Generic;
using System.Linq;
using Abp.Configuration;
using Abp.Dependency;
using Abp.Domain.Uow;
using Castle.Core.Logging;
using Magicodes.PayNotify;
using Magicodes.PayNotify.Builder;
using Microsoft.Extensions.Configuration;

namespace Magicodes.Pay.Abp.Configs
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

        protected List<IPaymentConfigAction> Actions { get; set; }
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
            Actions = _iocManager.ResolveAll<IPaymentConfigAction>()?.ToList();

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


            if (Actions != null)
            {
                foreach (var action in Actions)
                {
                    //
                    action.Build(LogAction).Wait();
                }
            }
           
            PayNotifyConfig(LogAction);
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

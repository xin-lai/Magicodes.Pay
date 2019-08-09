using System;
using System.Collections.Generic;
using System.Text;

namespace Magicodes.Allinpay.Builder
{
    /// <summary>
    ///  通联支付
    /// </summary>
    public class AllinpayBuilder
    {
        private Action<string, string> LoggerAction { get; set; }

        private Func<IAllinpaySettings> GetPayConfigFunc { get; set; }


        /// <summary>
        ///     创建实例
        /// </summary>
        /// <returns></returns>
        public static AllinpayBuilder Create()
        {
            return new AllinpayBuilder();
        }

        /// <summary>
        ///     设置日志记录处理
        /// </summary>
        /// <param name="loggerAction"></param>
        /// <returns></returns>
        public AllinpayBuilder WithLoggerAction(Action<string, string> loggerAction)
        {
            LoggerAction = loggerAction;
            return this;
        }

        /// <summary>
        ///     注册配置获取逻辑
        /// </summary>
        /// <param name="func"></param>
        /// <returns></returns>
        public AllinpayBuilder RegisterGetPayConfigFunc(Func<IAllinpaySettings> func)
        {
            GetPayConfigFunc = func;
            return this;
        }

        /// <summary>
        ///     确定设置
        /// </summary>
        public void Build()
        {
            if (LoggerAction != null)
                AllinpayAppService.LoggerAction = LoggerAction;

            if (GetPayConfigFunc != null)
                AllinpayAppService.GetPayConfigFunc = GetPayConfigFunc;
        }
    }
}

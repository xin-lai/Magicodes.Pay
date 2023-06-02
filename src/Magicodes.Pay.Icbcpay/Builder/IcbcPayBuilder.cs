using System;
using System.Collections.Generic;
using System.Text;

namespace Magicodes.Pay.Icbc.Builder
{
    public class IcbcpayBuilder
    {
        private Action<string, string> LoggerAction { get; set; }

        private Func<IIcbcpaySettings> GetPayConfigFunc { get; set; }

        /// <summary>
        ///     创建实例
        /// </summary>
        /// <returns></returns>
        public static IcbcpayBuilder Create()
        {
            return new IcbcpayBuilder();
        }

        /// <summary>
        ///     设置日志记录处理
        /// </summary>
        /// <param name="loggerAction"></param>
        /// <returns></returns>
        public IcbcpayBuilder WithLoggerAction(Action<string, string> loggerAction)
        {
            LoggerAction = loggerAction;
            return this;
        }

        /// <summary>
        ///     注册配置获取逻辑
        /// </summary>
        /// <param name="func"></param>
        /// <returns></returns>
        public IcbcpayBuilder RegisterGetPayConfigFunc(Func<IIcbcpaySettings> func)
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
                IcbcpayAppService.LoggerAction = LoggerAction;

            if (GetPayConfigFunc != null)
                IcbcpayAppService.GetPayConfigFunc = GetPayConfigFunc;
        }

    }
}

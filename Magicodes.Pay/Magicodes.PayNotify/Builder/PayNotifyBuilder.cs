using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Magicodes.PayNotify.Models;

namespace Magicodes.PayNotify.Builder
{
    public class PayNotifyBuilder
    {
        private Action<string, string> LoggerAction { get; set; }
        private Func<PayNotifyInput, Task<string>> PayNotifyFunc { get; set; }


        /// <summary>
        ///     创建实例
        /// </summary>
        /// <returns></returns>
        public static PayNotifyBuilder Create() => new PayNotifyBuilder();

        /// <summary>
        ///     设置日志记录处理
        /// </summary>
        /// <param name="loggerAction"></param>
        /// <returns></returns>
        public PayNotifyBuilder WithLoggerAction(Action<string, string> loggerAction)
        {
            LoggerAction = loggerAction;
            return this;
        }

        /// <summary>
        /// 设置支付回调处理逻辑
        /// </summary>
        /// <param name="payNotifyFunc"></param>
        /// <returns></returns>
        public PayNotifyBuilder WithPayNotifyFunc(Func<PayNotifyInput, Task<string>> payNotifyFunc)
        {
            PayNotifyFunc = payNotifyFunc;
            return this;
        }

        /// <summary>
        ///     确定设置
        /// </summary>
        public void Build()
        {
            if (LoggerAction != null)
                PayNotifyController.LoggerAction = LoggerAction;

            if (PayNotifyFunc != null)
                PayNotifyController.PayNotifyFunc = PayNotifyFunc;

            
        }
    }
}

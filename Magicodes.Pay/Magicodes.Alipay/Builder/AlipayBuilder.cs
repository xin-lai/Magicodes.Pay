using System;
using System.Collections.Generic;
using System.Text;

namespace Magicodes.Alipay.Builder
{
    public class AlipayBuilder
    {
        private Action<string, string> LoggerAction { get; set; }

        private Func<IAlipaySettings> GetPayConfigFunc { get; set; }


        /// <summary>
        ///     创建实例
        /// </summary>
        /// <returns></returns>
        public static AlipayBuilder Create() => new AlipayBuilder();

        /// <summary>
        ///     设置日志记录处理
        /// </summary>
        /// <param name="loggerAction"></param>
        /// <returns></returns>
        public AlipayBuilder WithLoggerAction(Action<string, string> loggerAction)
        {
            LoggerAction = loggerAction;
            return this;
        }

        /// <summary>
        /// 注册配置获取逻辑
        /// </summary>
        /// <param name="func"></param>
        /// <returns></returns>
        public AlipayBuilder RegisterGetPayConfigFunc(Func<IAlipaySettings> func)
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
                AlipayAppService.LoggerAction = LoggerAction;

            if (GetPayConfigFunc != null)
                AlipayAppService.GetPayConfigFunc = GetPayConfigFunc;
        }
    }
}

// ======================================================================
//   
//           Copyright (C) 2018-2020 湖南心莱信息科技有限公司    
//           All rights reserved
//   
//           filename : PayNotifyBuilder.cs
//           description :
//   
//           created by 雪雁 at  2018-07-16 15:46
//           Mail: wenqiang.li@xin-lai.com
//           QQ群：85318032（技术交流）
//           Blog：http://www.cnblogs.com/codelove/
//           GitHub：https://github.com/xin-lai
//           Home：http://xin-lai.com
//   
// ======================================================================

using Magicodes.Pay.Notify.Models;

namespace Magicodes.Pay.Notify.Builder
{
    public class PayNotifyBuilder
    {
        private Action<string, string> LoggerAction { get; set; }
        private Func<PayNotifyInput, Task<string>> PayNotifyFunc { get; set; }


        /// <summary>
        ///     创建实例
        /// </summary>
        /// <returns></returns>
        public static PayNotifyBuilder Create()
        {
            return new PayNotifyBuilder();
        }

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
        ///     设置支付回调处理逻辑
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
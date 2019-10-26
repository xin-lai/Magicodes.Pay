// ======================================================================
//   
//           Copyright (C) 2018-2020 湖南心莱信息科技有限公司    
//           All rights reserved
//   
//           filename : WeChatPayBuilder.cs
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

using System;
using Magicodes.Pay.Wxpay.Config;
using Magicodes.Pay.Wxpay.Helper;

namespace Magicodes.Pay.Wxpay.Builder
{
    /// <summary>
    ///     WeChatSDK构造函数类
    /// </summary>
    public class WeChatPayBuilder
    {
        private Action<string, string> LoggerAction { get; set; }

        private Func<IWeChatPayConfig> GetPayConfigFunc { get; set; }

        /// <summary>
        ///     创建实例
        /// </summary>
        /// <returns></returns>
        public static WeChatPayBuilder Create()
        {
            return new WeChatPayBuilder();
        }

        /// <summary>
        ///     设置日志记录处理
        /// </summary>
        /// <param name="loggerAction"></param>
        /// <returns></returns>
        public WeChatPayBuilder WithLoggerAction(Action<string, string> loggerAction)
        {
            LoggerAction = loggerAction;
            return this;
        }

        /// <summary>
        ///     注册配置获取逻辑
        /// </summary>
        /// <param name="func"></param>
        /// <returns></returns>
        public WeChatPayBuilder RegisterGetPayConfigFunc(Func<IWeChatPayConfig> func)
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
                WeChatPayHelper.LoggerAction = LoggerAction;

            if (GetPayConfigFunc != null)
                WeChatPayHelper.GetPayConfigFunc = GetPayConfigFunc;
        }
    }
}
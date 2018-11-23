// ======================================================================
//   
//           Copyright (C) 2018-2020 湖南心莱信息科技有限公司    
//           All rights reserved
//   
//           filename : GlobalAlipayBuilder.cs
//           description :
//   
//           created by 雪雁 at  2018-11-23 9:07
//           Mail: wenqiang.li@xin-lai.com
//           QQ群：85318032（技术交流）
//           Blog：http://www.cnblogs.com/codelove/
//           GitHub：https://github.com/xin-lai
//           Home：http://xin-lai.com
//   
// ======================================================================

using System;

namespace Magicodes.Alipay.Global.Builder
{
    /// <summary>
    ///     Defines the <see cref="GlobalAlipayBuilder" />
    /// </summary>
    public class GlobalAlipayBuilder
    {
        /// <summary>
        ///     Gets or sets the LoggerAction
        /// </summary>
        private Action<string, string> LoggerAction { get; set; }

        /// <summary>
        ///     Gets or sets the GetPayConfigFunc
        /// </summary>
        private Func<IGlobalAlipaySettings> GetPayConfigFunc { get; set; }

        /// <summary>
        ///     创建实例
        /// </summary>
        /// <returns></returns>
        public static GlobalAlipayBuilder Create()
        {
            return new GlobalAlipayBuilder();
        }

        /// <summary>
        ///     设置日志记录处理
        /// </summary>
        /// <param name="loggerAction"></param>
        /// <returns></returns>
        public GlobalAlipayBuilder WithLoggerAction(Action<string, string> loggerAction)
        {
            LoggerAction = loggerAction;
            return this;
        }

        /// <summary>
        ///     注册配置获取逻辑
        /// </summary>
        /// <param name="func"></param>
        /// <returns></returns>
        public GlobalAlipayBuilder RegisterGetPayConfigFunc(Func<IGlobalAlipaySettings> func)
        {
            GetPayConfigFunc = func;
            return this;
        }

        /// <summary>
        ///     确定设置
        /// </summary>
        public void Build()
        {
            if (LoggerAction != null) GlobalAlipayAppService.LoggerAction = LoggerAction;

            if (GetPayConfigFunc != null) GlobalAlipayAppService.GetPayConfigFunc = GetPayConfigFunc;
        }
    }
}
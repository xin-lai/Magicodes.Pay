// ======================================================================
//  
//          Copyright (C) 2016-2020 湖南心莱信息科技有限公司    
//          All rights reserved
//  
//          filename : WeChatSDKBuilder.cs
//          description :
//  
//          created by 李文强 at  2016/10/04 20:16
//          Blog：http://www.cnblogs.com/codelove/
//          GitHub：https://github.com/xin-lai
//          Home：http://xin-lai.com
//  
// ======================================================================


using System;
using System.Collections.Generic;
using Magicodes.Pay.WeChat.Config;

namespace Magicodes.Pay.WeChat.Builder
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
        public static WeChatPayBuilder Create() => new WeChatPayBuilder();

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
        /// 注册配置获取逻辑
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
                WeChatPayApi.LoggerAction = LoggerAction;

            if (GetPayConfigFunc != null)
                WeChatPayApi.GetPayConfigFunc = GetPayConfigFunc;
        }
    }
}
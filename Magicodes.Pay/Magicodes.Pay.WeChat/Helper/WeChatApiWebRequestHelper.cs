// ======================================================================
//  
//          Copyright (C) 2016-2020 湖南心莱信息科技有限公司    
//          All rights reserved
//  
//          filename : WeChatApiWebRequestHelper.cs
//          description :
//  
//          created by 李文强 at  2018/04/10 17:10
//          Blog：http://www.cnblogs.com/codelove/
//          GitHub ： https://github.com/xin-lai
//          Home：http://xin-lai.com
//  
// ======================================================================

namespace Magicodes.Pay.WeChat.Helper
{
    public class WeChatApiWebRequestHelper : WebRequestHelper
    {
        public WeChatApiWebRequestHelper()
        {
            UserAgent =
                "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/31.0.1650.57 Safari/537.36";
            ContentType = "application/json";
            AcceptLanguage = "zh-cn";
        }
    }
}
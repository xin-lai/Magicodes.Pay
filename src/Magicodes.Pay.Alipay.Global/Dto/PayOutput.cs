// ======================================================================
//  
//          Copyright (C) 2018-2068 湖南心莱信息科技有限公司    
//          All rights reserved
//  
//          filename : PayOutput.cs
//          description :
//  
//          created by codelove1314@live.cn at  2018-11-23 09:39:31
//          Blog：http://www.cnblogs.com/codelove/
//          GitHub ： https://github.com/xin-lai
//          Home：http://xin-lai.com
//  
// =======================================================================

using System.Collections.Generic;

namespace Magicodes.Pay.Alipay.Global.Dto
{
    /// <summary>
    /// Defines the <see cref="PayOutput" />
    /// </summary>
    public class PayOutput
    {
        /// <summary>
        /// 参数列表
        /// </summary>
        public Dictionary<string,string> Parameters { get; set; }

        /// <summary>
        /// 表单Html
        /// </summary>
        public string FormHtml { get; set; }
    }
}

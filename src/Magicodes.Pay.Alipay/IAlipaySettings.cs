// ======================================================================
//   
//           Copyright (C) 2018-2020 湖南心莱信息科技有限公司    
//           All rights reserved
//   
//           filename : IAlipaySettings.cs
//           description :
//   
//           created by 雪雁 at  2018-07-16 15:53
//           Mail: wenqiang.li@xin-lai.com
//           QQ群：85318032（技术交流）
//           Blog：http://www.cnblogs.com/codelove/
//           GitHub：https://github.com/xin-lai
//           Home：http://xin-lai.com
//   
// ======================================================================

namespace Magicodes.Pay.Alipay
{
    /// <summary>
    /// 
    /// </summary>
    public interface IAlipaySettings
    {
        string AlipayPublicKey { get; set; }
        string AlipaySignPublicKey { get; set; }
        string AppId { get; set; }
        string CharSet { get; set; }
        string GatewayUrl { get; set; }
        bool IsKeyFromFile { get; set; }
        string Notify { get; set; }
        string PrivateKey { get; set; }
        string SignType { get; set; }
        string Uid { get; set; }
    }
}
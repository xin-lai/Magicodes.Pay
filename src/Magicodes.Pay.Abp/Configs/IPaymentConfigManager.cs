// ======================================================================
// 
//           Copyright (C) 2019-2030 湖南心莱信息科技有限公司
//           All rights reserved
// 
//           filename : IPaymentConfigManager.cs
//           description :
// 
//           created by 雪雁 at  -- 
//           文档官网：https://docs.xin-lai.com
//           公众号教程：麦扣聊技术
//           QQ群：85318032（编程交流）
//           Blog：http://www.cnblogs.com/codelove/
// 
// ======================================================================

using Abp;
using Abp.Dependency;

namespace Magicodes.Pay.Abp.Configs
{
    /// <summary>
    /// 支付配置管理器
    /// </summary>
    public interface IPaymentConfigManager : ISingletonDependency, IShouldInitialize
    {

    }
}
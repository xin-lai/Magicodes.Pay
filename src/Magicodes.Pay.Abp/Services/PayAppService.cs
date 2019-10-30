// ======================================================================
// 
//           Copyright (C) 2019-2030 湖南心莱信息科技有限公司
//           All rights reserved
// 
//           filename : PayAppService.cs
//           description :
// 
//           created by 雪雁 at  -- 
//           文档官网：https://docs.xin-lai.com
//           公众号教程：麦扣聊技术
//           QQ群：85318032（编程交流）
//           Blog：http://www.cnblogs.com/codelove/
// 
// ======================================================================

using System.Threading.Tasks;
using Abp.Dependency;
using Abp.UI;
using Magicodes.Pay.Abp.Services.Dto;

namespace Magicodes.Pay.Abp.Services
{
    /// <summary>
    ///  统一支付服务
    /// </summary>
    public class PayAppService : IPayAppService, ITransientDependency
    {
        private readonly IPaymentManager _paymentManager;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="paymentManager"></param>
        public PayAppService(IPaymentManager paymentManager)
        {
            this._paymentManager = paymentManager;
        }

        /// <summary>
        /// 统一支付（支持各个渠道）
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public virtual async Task<object> Pay(PayInputBase input)
        {
            var service = await _paymentManager.GetPayService(input.PayChannel);
            if (service == null) throw new UserFriendlyException($"支付渠道 {input.PayChannel} 不存在，请确认是否已注册或已添加引用！");
            return await service.ToPay(input);
        }
    }
}
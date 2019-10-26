using System.Threading.Tasks;
using Abp.Application.Services;
using Magicodes.Pay.Abp.Services.Dto;

namespace Magicodes.Pay.Abp.Services
{

    public interface IPayAppService : IApplicationService
    {
        ///// <summary>
        ///// 阿里支付
        ///// </summary>
        ///// <param name="input"></param>
        ///// <returns></returns>
        //Task<string> AliAppPay(AppPayInput input);

        ///// <summary>
        ///// 微信支付
        ///// </summary>
        ///// <param name="input"></param>
        ///// <returns></returns>
        //Task<WeChat.Pay.Dto.AppPayOutput> WeChatAppPay(AppPayInput input);

        /// <summary>
        /// 统一支付
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<object> Pay(PayInput input);

        ///// <summary>
        ///// 余额支付
        ///// </summary>
        ///// <param name="input"></param>
        ///// <returns></returns>
        //Task BalancePay(PayInput input);
    }
}
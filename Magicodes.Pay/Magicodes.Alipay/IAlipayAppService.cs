using System.Threading.Tasks;
using Magicodes.Alipay.Dto;

namespace Magicodes.Alipay
{
    /// <summary>
    /// 支付宝支付
    /// </summary>
    public interface IAlipayAppService
    {
        /// <summary>
        /// APP支付
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<AppPayOutput> AppPay(AppPayInput input);

        /// <summary>
        /// WAP支付
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<WapPayOutput> WapPay(WapPayInput input);
    }
}
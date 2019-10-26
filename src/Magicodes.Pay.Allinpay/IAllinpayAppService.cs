using Magicodes.Pay.Allinpay.Dto;

namespace Magicodes.Pay.Allinpay
{
    public interface IAllinpayAppService
    {
        Task<WeChatMiniPayOutput> WeChatMiniPay(WeChatMiniPayInput input);

        bool PayNotifyHandler(Dictionary<string, string> dic);
    }
}

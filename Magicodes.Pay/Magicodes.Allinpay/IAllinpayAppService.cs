using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Magicodes.Allinpay.Dto;

namespace Magicodes.Allinpay
{
    public interface IAllinpayAppService
    {
        Task<WeChatMiniPayOutput> WeChatMiniPay(WeChatMiniPayInput input);
    }
}

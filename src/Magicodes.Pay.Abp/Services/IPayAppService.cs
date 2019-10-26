using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Magicodes.Pay.Abp.Services.Dto;

namespace Magicodes.Pay.Abp.Services
{
    /// <summary>
    /// 统一支付服务
    /// </summary>
    public interface IPayAppService
    {
        /// <summary>
        /// 统一支付
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<object> Pay(PayInputBase input);
    }
}

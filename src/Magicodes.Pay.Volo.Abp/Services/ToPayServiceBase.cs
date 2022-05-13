using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Abp.Dependency;
using Magicodes.Pay.Volo.Abp.Services.Dto;
using Magicodes.Pay.Volo.Abp.TransactionLogs;

namespace Magicodes.Pay.Volo.Abp.Services
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class ToPayServiceBase : IToPayService
    {
        /// <summary>
        /// 
        /// </summary>
        protected IIocResolver IocResolver;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="iocResolver"></param>
        protected ToPayServiceBase(IIocResolver iocResolver)
        {
            IocResolver = iocResolver;
        }

        /// <summary>
        /// 支付渠道
        /// </summary>
        public virtual PayChannels PayChannel { get; }

        /// <summary>
        /// 支付
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public virtual Task<object> ToPay(PayInputBase input)
        {
            throw new NotImplementedException();
        }
    }
}

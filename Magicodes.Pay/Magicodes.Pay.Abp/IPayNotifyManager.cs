using System.Threading.Tasks;
using Abp.Dependency;
using Magicodes.PayNotify.Models;

namespace Magicodes.Pay.Abp
{
    /// <summary>
    /// 支付回调通知管理器
    /// </summary>
    public interface IPayNotifyManager : ITransientDependency
    {
        /// <summary>
        /// 执行支付回调通知
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<string> ExecPayNotifyAsync(PayNotifyInput input);
    }
}
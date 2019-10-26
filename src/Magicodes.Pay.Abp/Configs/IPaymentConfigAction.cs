using System;
using System.Threading.Tasks;

namespace Magicodes.Pay.Abp.Configs
{
    /// <summary>
    /// 支付注册和配置逻辑
    /// </summary>
    public interface IPaymentConfigAction
    {
        /// <summary>
        /// 关键字
        /// </summary>
        string Key { get; set; }

        /// <summary>
        /// 构建支付配置
        /// </summary>
        /// <param name="logAction"></param>
        /// <returns></returns>
        Task Build(Action<string, string> logAction);
    }
}
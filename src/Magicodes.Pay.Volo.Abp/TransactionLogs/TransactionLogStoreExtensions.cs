// ======================================================================
//   
//           Copyright (C) 2018-2020 湖南心莱信息科技有限公司    
//           All rights reserved
//   
//           filename : TransactionLogStoreExtensions.cs
//           description :
//   
//           created by 雪雁 at  2018-08-06 14:21
//           Mail: wenqiang.li@xin-lai.com
//           QQ群：85318032（技术交流）
//           Blog：http://www.cnblogs.com/codelove/
//           GitHub：https://github.com/xin-lai
//           Home：http://xin-lai.com
//   
// ======================================================================

using Abp.Threading;

namespace Magicodes.Pay.Volo.Abp.TransactionLogs
{
    /// <summary>
    ///     扩展
    /// </summary>
    public static class TransactionLogStoreExtensions
    {
        /// <summary>
        ///     保存交易日志
        /// </summary>
        /// <param name="transactionLogStore"></param>
        /// <param name="transactionLog"></param>
        public static void Save(this ITransactionLogStore transactionLogStore, TransactionLog transactionLog)
        {
            AsyncHelper.RunSync(() => transactionLogStore.SaveAsync(transactionLog));
        }
    }
}
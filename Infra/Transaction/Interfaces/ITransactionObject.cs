using System;
using System.Threading.Tasks;

namespace SampleApiServer.Infra.Transaction
{
    /// <summary>
    /// TransactionObjectを表すインターフェース
    /// </summary>
    public interface ITransactionObject : IDisposable
    {
        /// <summary>
        /// トランザクションをコミットする
        /// </summary>
        Task Commit();

        /// <summary>
        /// トランザクションをロールバックする
        /// </summary>
        void Rollback();
    }
}

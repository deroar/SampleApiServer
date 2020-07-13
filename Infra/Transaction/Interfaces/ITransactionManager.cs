namespace SampleApiServer.Infra.Transaction
{
    /// <summary>
    /// トランザクション管理処理のインターフェース。
    /// </summary>
    public interface ITransactionManager
    {
        /// <summary>
        /// トランザクションを開始する。
        /// </summary>
        ITransactionObject BeginTransaction();
    }
}

using SampleApiServer.Infra.DbContexts;

namespace SampleApiServer.Infra.Transaction
{
    /// <summary>
    /// トランザクションを管理するクラス。
    /// </summary>
    /// <remarks>
    /// </remarks>
    public class TransactionManager : ITransactionManager
    {
        #region メンバー変数

        /// <summary>
        /// プレイヤーDBコンテキスト。
        /// </summary>
        private readonly PlayerBoundDbContext playerBoundDbContext;

        /// <summary>
        /// 現在使用中のトランザクション。
        /// </summary>
        private TransactionObject transactionObject;

        #endregion

        #region コンストラクタ

        /// <summary>
        /// トランザクション管理対象のDBコンテキストからインスタンスを生成する。
        /// </summary>
        /// <param name="playerBoundDbContext">プレイヤーDBコンテキスト。</param>
        public TransactionManager(PlayerBoundDbContext playerBoundDbContext)
        {
            this.playerBoundDbContext = playerBoundDbContext;
        }

        #endregion

        #region メソッド

        /// <summary>
        /// トランザクションを開始する。
        /// </summary>
        public ITransactionObject BeginTransaction()
        {
            if (this.transactionObject == null)
            {
                // （このTrasactionObjcetがDisposeされたら、参照を手放す）
                this.transactionObject = new TransactionObject();
                this.transactionObject.OnDispose += () =>
                {
                    this.transactionObject = null;
                };

                this.transactionObject.TrackTransactionAsync(this.playerBoundDbContext).Wait();
            }

            return this.transactionObject;
        }

        #endregion
    }
}

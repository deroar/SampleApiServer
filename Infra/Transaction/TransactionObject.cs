using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using SampleApiServer.Exceptions;
using SampleApiServer.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SampleApiServer.Infra.Transaction
{
    /// <summary>
    /// トランザクションを提供するクラス。
    /// </summary>
    /// <remarks>複数のDBコンテキストを横断するトランザクションを提供する。</remarks>
    public class TransactionObject : ITransactionObject
    {
        #region メンバー変数

        /// <summary>
        /// 追跡中のトランザクション。
        /// </summary>
        private readonly IDictionary<DbContext, IDbContextTransaction> trackingTransactions;

        /// <summary>
        /// コミットもしくはロールバックが行われたか？
        /// </summary>
        private bool statusFixed = false;

        #endregion

        #region コンストラクタ

        /// <summary>
        /// トランザクション用のインスタンスを生成する。
        /// </summary>
        /// <remarks>トランザクション毎に新しいインスタンスを生成する必用有。</remarks>
        public TransactionObject()
        {
            this.trackingTransactions = new Dictionary<DbContext, IDbContextTransaction>();
        }

        #endregion

        #region イベント

        /// <summary>
        /// Dispose時に呼ばれるイベント。
        /// </summary>
        public event Action OnDispose;

        #endregion

        #region プロパティ

        /// <summary>
        /// 追跡しているEntityのうちEntityBaseを継承しているもの。
        /// </summary>
        private IEnumerable<EntityBase> TrackingEntites =>
            trackingTransactions.Keys
            .Select(dbContext => dbContext.ChangeTracker)
            .SelectMany(tracker => tracker.Entries())
            .Select(entry => entry.Entity)
            .OfType<EntityBase>();

        #endregion

        #region 公開メソッド

        /// <summary>
        /// トランザクションをコミットする。
        /// </summary>
        /// <returns>処理状態。</returns>
        public async Task Commit()
        {
            if (statusFixed) { return; }
            statusFixed = true;

            bool noCommit = true;
            List<Exception> innerExcepions = null;

            foreach (var tran in trackingTransactions.Values)
            {
                try
                {
                    tran.Commit();
                }
                catch (Exception e)
                {
                    // 最初のコミットが失敗したときはTrasactionとしては不整合はないので、単に例外を投げるだけでいい
                    if (noCommit)
                    {
                        throw e;
                    }

                    // すでに1つ以上のコミットを実行してしまった場合は、仕方ないのでできるだけcommitしようとする
                    if (innerExcepions == null)
                    {
                        innerExcepions = new List<Exception>();
                    }

                    innerExcepions.Add(e);
                }

                noCommit = false;
            }

            // いくつかのコミットだけ失敗してしまった場合は、内部のexceptionをまとめた上でNGEServerExceptionとする
            if (innerExcepions != null)
            {
                var ae = new AggregateException(innerExcepions);
                throw new ServerException(ErrorCode.UNKNOWN,
                    "The transaction was broken between databases.", ae);
            }
            else
            {
                await Task.WhenAll(TrackingEntites.Select(entity => entity.SubmitChanges()));
            }
        }

        /// <summary>
        /// トランザクションをロールバックする
        /// </summary>
        public void Rollback()
        {
            if (statusFixed) { return; }
            statusFixed = true;

            foreach (var tran in trackingTransactions.Values)
            {
                tran.Rollback();
            }
        }

        /// <summary>
        /// 渡されたDBコンテキストのトランザクションを管理対象に追加する。
        /// </summary>
        /// <param name="context">トランザクションを行うDBコンテキスト。</param>
        /// <returns>処理状態。</returns>
        public async Task TrackTransactionAsync(DbContext context)
        {
            if (!trackingTransactions.ContainsKey(context))
            {
                var tran = await context.Database.BeginTransactionAsync();
                trackingTransactions.Add(context, tran);
            }
        }

        #endregion

        #region 以下、VisualStudio の IDisposable Support による生成

        private bool disposedValue = false;

        /// <summary>
        /// Disposeの実装
        /// </summary>
        /// <param name="disposing">Dispose中か？</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // もしこの時点で状態が決定していなければRollbackが実行される
                    this.Rollback();

                    foreach (var tran in trackingTransactions.Values)
                    {
                        tran.Dispose();
                    }

                    this.OnDispose?.Invoke();
                }

                disposedValue = true;
            }
        }

        // このコードは、破棄可能なパターンを正しく実装できるように追加されました。
        /// <summary>
        /// Disposeの実装
        /// </summary>
        public void Dispose()
        {
            // このコードを変更しないでください。クリーンアップ コードを上の Dispose(bool disposing) に記述します。
            Dispose(true);
        }

        #endregion
    }
}

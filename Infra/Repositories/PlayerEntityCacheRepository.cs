using Microsoft.Extensions.Hosting;
using SampleApiServer.Infra.Models;
using SampleApiServer.Models.Entities;
using System;

namespace SampleApiServer.Infra.Repositories
{
    /// <summary>
    /// プレイヤー系エンティティ用のキャッシュリポジトリ。
    /// </summary>
    /// <typeparam name="TEntity">キャッシュするエンティティクラス。</typeparam>
    public class PlayerEntityCacheRepository<TEntity> : RedisCacheRepository<TEntity>
        where TEntity : EntityBase
    {
        #region 定数

        /// <summary>
        /// キャッシュ保持期間。
        /// </summary>
        /// <remarks>プレイヤー情報なので、30分程度の短めで設定。</remarks>
        private readonly TimeSpan? expiration = new TimeSpan(0, 30, 0);

        #endregion

        #region コンストラクタ

        /// <summary>
        /// 引き数をDIしてリポジトリを生成する。
        /// </summary>
        /// <param name="redis">Redis接続オブジェクト。</param>
        /// <param name="env">ホスティング環境情報。</param>
        public PlayerEntityCacheRepository(EphemeralRedis redis, IHostEnvironment env) : base(redis, env)
        {
        }

        #endregion

        #region 内部メソッド

        /// <summary>
        /// キャッシュの保持期間を取得する。
        /// </summary>
        /// <returns>保持期間。</returns>
        protected override TimeSpan? GetExpiration()
        {
            return this.expiration;
        }

        #endregion
    }
}

using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using SampleApiServer.Exceptions;
using SampleApiServer.Infra.Models;
using SampleApiServer.Models.Entities;
using SampleApiServer.Util.Json;
using System;
using System.Threading.Tasks;

namespace SampleApiServer.Infra.Repositories
{
    /// <summary>
    /// ICacheRepositoryのRedisによる実装。
    /// </summary>
    /// <typeparam name="TEntity">キャッシュするエンティティクラス。</typeparam>
    /// <remarks>
    /// 汎用のRedisキャッシュクラス。キャッシュ保持期間など調整する場合は、サブクラスで実装をカスタマイズしてください。
    /// </remarks>
    public class RedisCacheRepository<TEntity> : RedisOperationRepositoryBase, ICacheRepository<TEntity>
        where TEntity : EntityBase
    {
        #region 定数

        /// <summary>
        /// データ名。
        /// </summary>
        private const string DATA_NAME = "Cache";

        #endregion

        #region コンストラクタ

        /// <summary>
        /// 引き数をDIしてリポジトリを生成する。
        /// </summary>
        /// <param name="redis">Redis接続オブジェクト。</param>
        /// <param name="env">ホスティング環境情報。</param>
        public RedisCacheRepository(EphemeralRedis redis, IHostEnvironment env)
            : base(DATA_NAME, env.EnvironmentName, redis.Connection)
        {
        }

        #endregion

        #region 公開メソッド

        /// <summary>
        /// キーに基づいてキャッシュを取得する
        /// </summary>
        /// <param name="keys">Entityを特定するキー</param>
        /// <returns>Entity</returns>
        public async Task<TEntity> GetOrNull(params object[] keys)
        {
            string cacheKey = MakeCacheKey(keys);
            TEntity entity = null;
            try
            {
                string serialized = await Get(cacheKey);
                if (serialized != null)
                {
                    entity = Deserialize(serialized);
                    // Deserialize 中に property のセットによって HasChange フラグが立ってしまうので、一回折っておく
                    entity.HasChange = false;
                }
            }
            catch (Exception e)
            {
                // エラー出力
                throw  new ServerException(ErrorCode.NOT_FOUND, e.Message);
            }
            return entity;
        }

        /// <summary>
        /// シリアライズされたEntityを得る
        /// </summary>
        /// <param name="entity">Entity</param>
        /// <returns>Entityのシリアライズ表現文字列</returns>
        public string GetSerialized(TEntity entity)
        {
            try
            {
                return Serialize(entity);
            }
            catch (Exception e)
            {
                // エラー出力
                throw new ServerException(ErrorCode.NOT_FOUND, e.Message);
            }
        }

        /// <summary>
        /// Entityをキャッシュする
        /// </summary>
        /// <param name="entity">Entity</param>
        /// <param name="keys">Entityを特定するキー</param>
        /// <returns>処理状態。</returns>
        public async Task SetCache(TEntity entity, params object[] keys)
        {
            if (entity == null)
            {
                return;
            }
            try
            {
                string serialized = Serialize(entity);
                await SetCache(serialized, keys);
            }
            catch (Exception e)
            {
                // エラー出力
                throw new ServerException(ErrorCode.NOT_FOUND, e.Message);
            }
        }

        /// <summary>
        /// Entityをキャッシュする
        /// </summary>
        /// <param name="serializedEntity">シリアライズ済のEntity</param>
        /// <param name="keys">Entityを特定するキー</param>
        /// <returns>処理状態。</returns>
        public async Task SetCache(string serializedEntity, params object[] keys)
        {
            if (serializedEntity == null)
            {
                return;
            }

            string cacheKey = MakeCacheKey(keys);
            var expiration = this.GetExpiration();
            try
            {
                await this.Set(cacheKey, serializedEntity, expiration);
            }
            catch (Exception e)
            {
                // エラー出力
                throw new ServerException(ErrorCode.NOT_FOUND, e.Message);
            }
        }

        /// <summary>
        /// Entityのキャッシュをクリアする
        /// </summary>
        /// <param name="keys">Entityを特定するキー</param>
        /// <returns>処理状態。</returns>
        public async Task Clear(params object[] keys)
        {
            string cacheKey = MakeCacheKey(keys);
            try
            {
                await DeleteKey(cacheKey);
            }
            catch (Exception e)
            {
                // エラー出力
                throw new ServerException(ErrorCode.NOT_FOUND, e.Message);
            }
        }

        #endregion

        #region 内部メソッド

        /// <summary>
        /// キャッシュの保持期間を取得する。
        /// </summary>
        /// <returns>保持期間。無期限の場合null。</returns>
        /// <remarks>
        /// このクラスでは常にnullを返します。
        /// 用途に合わせて継承クラスで書き換えてください。
        /// </remarks>
        protected virtual TimeSpan? GetExpiration()
        {
            return null;
        }

        /// <summary>
        /// 保存可能な形式にシリアライズする。
        /// </summary>
        /// <param name="entity">シリアライズするエンティティ。</param>
        /// <returns>シリアライズしたJSON。</returns>
        private string Serialize(TEntity entity)
        {
            return JsonConvert.SerializeObject(entity, new IgnoreNotMappedSerializerSettings());
        }

        /// <summary>
        /// 保存可能な形式からデシリアライズする。
        /// </summary>
        /// <param name="value">デシリアライズするJSON。</param>
        /// <returns>デシリアライズしたエンティティ。</returns>
        private TEntity Deserialize(string value)
        {
            return JsonConvert.DeserializeObject<TEntity>(value, new PrivatePropWritingSerializerSettings());
        }

        /// <summary>
        /// cache用のキーを作成する。
        /// </summary>
        /// <param name="keys">キーの条件値。</param>
        /// <returns>生成したキー文字列。</returns>
        private string MakeCacheKey(params object[] keys)
        {
            if (keys == null || keys.Length <= 0)
            {
                throw new ArgumentNullException(nameof(keys));
            }

            // typeof(TEntity).Name も含めて引数いっぺんに渡すとオーバーロード違いで変なことになるのでJoin2回読んでる
            string joinedPKeys = string.Join(Separator, keys);
            return string.Join(Separator, typeof(TEntity).Name, joinedPKeys);
        }

        #endregion
    }
}

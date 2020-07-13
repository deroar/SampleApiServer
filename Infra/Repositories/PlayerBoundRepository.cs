using Microsoft.EntityFrameworkCore;
using SampleApiServer.Exceptions;
using SampleApiServer.Infra.DbContexts;
using SampleApiServer.Infra.Util.Extensions;
using SampleApiServer.Models.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SampleApiServer.Infra.Repositories
{
    /// <summary>
    /// プレイヤー系エンティティ用の汎用リポジトリ。
    /// </summary>
    /// <typeparam name="TEntity">プレイヤー系エンティティ。</typeparam>
    public class PlayerBoundRepository<TEntity> : IPlayerBoundRepository<TEntity>
        where TEntity : PlayerBoundEntityBase
    {
        #region メンバ変数

        /// <summary>
        /// DBコンテキスト
        /// </summary>
        protected readonly PlayerBoundDbContext dbContext;

        /// <summary>
        /// キャッシュリポジトリ
        /// </summary>
        protected readonly ICacheRepository<TEntity> cacheRepository;

        /// <summary>
        /// DIされるIServiceProvider
        /// </summary>
        protected readonly IServiceProvider serviceProvider;

        #endregion

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="dbContext">DBコンテキスト</param>
        public PlayerBoundRepository(
            PlayerBoundDbContext dbContext,
            ICacheRepository<TEntity> cacheRepository,
            IServiceProvider serviceProvider
            )
        {
            this.dbContext = dbContext;
            this.cacheRepository = cacheRepository;
            this.serviceProvider = serviceProvider;
        }

        #region 更新系メソッド

        /// <summary>
        /// Entityを新規に作成する
        /// </summary>
        /// <param name="entity">作成対象のEntity</param>
        /// <returns>作成後のEntity。AutoGenerateのIDを含む</returns>
        public async Task<TEntity> CreateAsync(TEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            await dbContext.Set<TEntity>().AddAsync(entity);
            entity.PrepareToSubmit(null);
            await SaveChangesAsync(dbContext, entity);
            return entity;
        }

        /// <summary>
        /// Entityを更新する
        /// </summary>
        /// <param name="entity">更新対象のEntity</param>
        /// <returns>Entity</returns>
        public async Task<TEntity> UpdateAsync(TEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }
            if (!entity.HasChange)
            {
                return entity;
            }
            if (entity.IsFirstCommit)
            {
                return await CreateAsync(entity);
            }

            dbContext.Update(entity);
            try
            {
                entity.PrepareToSubmit(null);
                await SaveChangesAsync(dbContext, entity);
                return entity;
            }
            catch (DbUpdateConcurrencyException)
            {
                // Concurrencyの競合で失敗した場合、キャッシュが古い可能性があるので削除しておく
                throw;
            }
        }

        #endregion

        #region 参照系メソッド

        /// <summary>
        /// Keyを指定してEntityを取得する
        /// </summary>
        /// <param name="playerId">PlayerId</param>
        /// <param name="keys">Primary Key</param>
        /// <returns>Entity</returns>
        public async Task<TEntity> FindAsync(long playerId, params object[] keys)
        {
            TEntity entity = await FindOrNullAsync(playerId, keys);
            if (entity == null)
            {
                throw new NotFoundException(
                    $"Entity not found. entity: {typeof(TEntity).Name}, playerId: {playerId}, keys: {string.Join(',', keys)}");
            }
            return entity;
        }

        /// <summary>
        /// Keyを指定してEntityを取得する。存在しなかった場合nullを返す
        /// </summary>
        /// <param name="playerId">PlayerId</param>
        /// <param name="keys">Primary Key</param>
        /// <returns>Entity</returns>
        public async Task<TEntity> FindOrNullAsync(long playerId, params object[] keys)
        {
            // EFは引数と主キー数が合っているかチェックするため、すべての可変長引数をフラットに渡せるように新しい配列に詰め替える
            object[] paramsToFind = RepackPrimaryKeys(dbContext, keys, playerId);

            TEntity entity = dbContext.FindLocal<TEntity>(paramsToFind);
            if (entity != null) // メモリにあった
            {
                return entity.InjectDependencies(serviceProvider);
            }

            // includeすべきリレーションを持っているEntityはキャッシュしない
            bool canUseCache = !dbContext.HasRelationProperty<TEntity>();
            if (canUseCache)
            {
                entity = await cacheRepository.GetOrNull(paramsToFind);
                if (entity != null) // キャッシュヒット
                {
                    dbContext.Attach(entity);
                    return entity.InjectDependencies(serviceProvider);
                }
            }

            entity = await dbContext.BuildShallowIncludingQuery<TEntity>()
                .FirstOrDefaultAsync(dbContext.BuildPredicateToFind<TEntity>(paramsToFind));
            if (entity != null)
            {
                entity = entity.InjectDependencies(serviceProvider);
                if (canUseCache)
                {
                    // キャッシュになかったものが見つかったらキャッシュしておく
                    await cacheRepository.SetCache(entity, dbContext.FindPrimaryKeyValues(entity));
                }
            }
            return entity;
        }

        #endregion

        #region 削除系

        /// <summary>
        /// Entityを削除する
        /// </summary>
        /// <param name="entity">削除対象のEntity</param>
        public async Task DeleteAsync(TEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            dbContext.Remove(entity);
            entity.HasChange = true;
            entity.PrepareToSubmit(null);
            await SaveChangesAsync(dbContext, entity);
        }

        #endregion

        #region メソッド

        /// <summary>
        /// Transactionの状態を考慮しつつSaveする
        /// </summary>
        /// <param name="dbContext">DBコンテキスト</param>
        /// <param name="entity">エンティティ</param>
        protected async Task SaveChangesAsync(DbContext dbContext, TEntity entity)
        {
            await SaveChangesAsync(dbContext, new List<TEntity> { entity });
        }

        /// <summary>
        /// Transactionの状態を考慮しつつSaveする
        /// </summary>
        /// <param name="dbContext">DBコンテキスト</param>
        /// <param name="entities">エンティティ</param>
        protected async Task SaveChangesAsync(DbContext dbContext, IEnumerable<TEntity> entities)
        {
            await dbContext.SaveChangesAsync();

            // CurrentTransactionがない ＝ 単体の変更の場合、entityのsubmitを即時呼んでいい
            // 外部のトランザクションにくるまれている場合は、commit時に呼ばれるのでそちらに任せる
            if (dbContext.Database.CurrentTransaction == null)
            {
                foreach (var e in entities)
                {
                    await e.SubmitChanges();
                }
            }
        }

        #endregion

        #region 内部メソッド

        // EntityのOnSubmitのコールバック用に、cacheをセットする関数を返す
        private Func<Task> SetCacheOnSubmit(DbContext dbContext, TEntity entity)
        {
            string serialized = cacheRepository.GetSerialized(entity);
            var keys = dbContext.FindPrimaryKeyValues(entity);
            return () => cacheRepository.SetCache(serialized, keys);
        }

        // EntityのOnSubmitのコールバック用に、cacheをクリアする関数を返す
        private Func<Task> ClearCacheOnSubmit(DbContext dbContext, TEntity entity)
        {
            var keys = dbContext.FindPrimaryKeyValues(entity);
            return () => cacheRepository.Clear(keys);
        }

        // PlayerIdを含めたKeyの配列を返す
        private object[] RepackPrimaryKeys(DbContext dbContext, object[] otherKeys, long playerId)
        {
            if (otherKeys.Length == 0)
            {
                return new object[] { playerId };
            }

            var repackedPrimaryKeys = new object[otherKeys.Length + 1];

            int playerIdKeyIndex = dbContext.FindPrimaryKeyIndex<TEntity, long>(p => p.PlayerId);
            for (int i = 0; i < playerIdKeyIndex; ++i)
            {
                repackedPrimaryKeys[i] = otherKeys[i];
            }
            repackedPrimaryKeys[playerIdKeyIndex] = playerId;
            for (int i = playerIdKeyIndex + 1; i < repackedPrimaryKeys.Length; ++i)
            {
                repackedPrimaryKeys[i] = otherKeys[i - 1];
            }
            return repackedPrimaryKeys;
        }

        #endregion
    }
}

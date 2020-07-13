using SampleApiServer.Models.Entities;
using System.Threading.Tasks;

namespace SampleApiServer.Infra.Repositories
{
    /// <summary>
    /// Cacheの機能を提供するRepositoryのインターフェース
    /// Infraのレイヤ以下でしか使わないため、このインターフェースもInfraに定義されている
    /// </summary>
    public interface ICacheRepository<TEntity>
        where TEntity : EntityBase
    {
        /// <summary>
        /// キーに基づいてキャッシュを取得する
        /// </summary>
        /// <param name="keys">Entityを特定するキー</param>
        /// <returns>Entity</returns>
        Task<TEntity> GetOrNull(params object[] keys);

        /// <summary>
        /// シリアライズされたEntityを得る
        /// </summary>
        /// <param name="entity">Entity</param>
        /// <returns>Entityのシリアライズ表現文字列</returns>
        string GetSerialized(TEntity entity);

        /// <summary>
        /// Entityをキャッシュする
        /// </summary>
        /// <param name="entity">Entity</param>
        /// <param name="keys">Entityを特定するキー</param>
        Task SetCache(TEntity entity, params object[] keys);

        /// <summary>
        /// Entityをキャッシュする
        /// </summary>
        /// <param name="serializedEntity">シリアライズ済のEntity</param>
        /// <param name="keys">Entityを特定するキー</param>
        Task SetCache(string serializedEntity, params object[] keys);

        /// <summary>
        /// Entityのキャッシュをクリアする
        /// </summary>
        /// <param name="keys">Entityを特定するキー</param>
        Task Clear(params object[] keys);
    }
}

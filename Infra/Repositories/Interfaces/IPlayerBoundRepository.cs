using SampleApiServer.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace SampleApiServer.Infra.Repositories
{
    /// <summary>
    /// プレイヤー情報のリポジトリを表すインターフェース。
    /// </summary>
    /// <typeparam name="T">プレイヤー系エンティティ。</typeparam>
    public interface IPlayerBoundRepository<T> where T : PlayerBoundEntityBase
    {
        /// <summary>
        /// Entityを新規に作成する
        /// </summary>
        /// <param name="entity">作成対象のEntity</param>
        /// <returns>作成後のEntity。AutoGenerateのIDを含む</returns>
        Task<T> CreateAsync(T entity);

        /// <summary>
        /// Keyを指定してEntityを取得する。存在しない場合はエラー
        /// </summary>
        /// <param name="playerId">プレイヤのID</param>
        /// <param name="otherKeys">他のPrimary Key</param>
        /// <returns>Entity</returns>
        /// <exception cref="NGENotFoundException">プレイヤーが存在しない場合。</exception>
        Task<T> FindAsync(long playerId, params object[] otherKeys);

        /// <summary>
        /// Keyを指定してEntityを取得する
        /// </summary>
        /// <param name="playerId">プレイヤのID</param>
        /// <param name="otherKeys">他のPrimary Key</param>
        /// <returns>Entity</returns>
        Task<T> FindOrNullAsync(long playerId, params object[] otherKeys);

        /// <summary>
        /// Entityを更新する
        /// </summary>
        /// <param name="entity">更新対象のEntity</param>
        /// <returns>Entity</returns>
        Task<T> UpdateAsync(T entity);
    }
}

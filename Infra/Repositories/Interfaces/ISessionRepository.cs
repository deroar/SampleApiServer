using SampleApiServer.Models;
using System.Threading.Tasks;

namespace SampleApiServer.Infra.Repositories
{
    /// <summary>
    /// セッション情報を保持するリポジトリ。
    /// </summary>
    public interface ISessionRepository
    {
        /// <summary>
        /// プレイヤーのセッション情報を取得する。
        /// </summary>
        /// <param name="playerId">プレイヤーID。</param>
        /// <returns>セッション情報。</returns>
        Task<Session> Get(long playerId);

        /// <summary>
        /// プレイヤーのセッション情報を登録する。
        /// </summary>
        /// <param name="playerId">プレイヤーID。</param>
        /// <param name="session">セッション情報。</param>
        /// <returns>処理状態。</returns>
        Task Set(long playerId, Session session);
    }
}

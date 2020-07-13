using System.Threading.Tasks;

namespace SampleApiServer.Infra.Repositories
{
    /// <summary>
    /// プレイヤー認証リポジトリインターフェース
    /// </summary>
    public interface IPlayerAuthenticationRepository
    {
        /// <summary>
        /// プレイヤーID設定
        /// </summary>
        /// <param name="playerId">プレイヤーID</param>
        /// <param name="sessionId">セッションID</param>
        Task SetPlayerIdAndSessionIdAsync(long playerId, string sessionId);

        /// <summary>
        /// プレイヤーID取得
        /// </summary>
        /// <returns>プレイヤーID</returns>
        Task<long> GetPlayerIdAsync();
    }
}

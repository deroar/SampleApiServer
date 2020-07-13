using Microsoft.AspNetCore.Http;
using SampleApiServer.Util;
using System.Threading.Tasks;

namespace SampleApiServer.Infra.Repositories
{
    /// <summary>
    /// プレイヤーの認証Cookieを扱うリポジトリ
    /// </summary>
    public class PlayerAuthenticationCookieRepository : IPlayerAuthenticationRepository
    {
        private readonly HttpContext httpContext;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="httpContextAccessor">DIされるHttpContextアクセサ</param>
        public PlayerAuthenticationCookieRepository(
            IHttpContextAccessor httpContextAccessor
        )
        {
            this.httpContext = httpContextAccessor.HttpContext;
        }

        /// <summary>
        /// プレイヤーIDとセッションID設定
        /// </summary>
        /// <param name="playerId">認証するプレイヤーID</param>
        /// <param name="sessionId">セッションID</param>
        /// <returns></returns>
        public async Task SetPlayerIdAndSessionIdAsync(long playerId, string sessionId)
        {
            await httpContext.SetPlayerIdAndSessionIdAsync(playerId, sessionId);
        }

        /// <summary>
        /// プレイヤーID取得
        /// </summary>
        /// <returns>プレイヤーID</returns>
        public async Task<long> GetPlayerIdAsync()
        {
            return await httpContext.GetPlayerIdAsync();
        }
    }
}

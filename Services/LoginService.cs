using SampleApiServer.Infra.Repositories;
using SampleApiServer.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SampleApiServer.Models;

namespace SampleApiServer.Services
{
    /// <summary>
    /// ログインサービス
    /// </summary>
    public class LoginService
    {
        private readonly IPlayerBoundRepository<PlayerAuth> playerAuthRepository;
        private readonly IPlayerBoundRepository<PlayerBasic> playerBasicRepository;
        private readonly IPlayerAuthenticationRepository playerAuthenticationRepository;
        private readonly ISessionRepository sessionRepository;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="playerAuthRepository">DI</param>
        /// <param name="playerBasicRepository">DI</param>
        /// <param name="playerAuthenticationRepository">DI</param>
        /// <param name="sessionRepository">DI</param>
        public LoginService(
            IPlayerBoundRepository<PlayerAuth> playerAuthRepository,
            IPlayerBoundRepository<PlayerBasic> playerBasicRepository,
            IPlayerAuthenticationRepository playerAuthenticationRepository,
            ISessionRepository sessionRepository
            )
        {
            this.playerAuthRepository = playerAuthRepository;
            this.playerBasicRepository = playerBasicRepository;
            this.playerAuthenticationRepository = playerAuthenticationRepository;
            this.sessionRepository = sessionRepository;
        }

        /// <summary>
        /// プレイヤーログイン
        /// </summary>
        /// <param name="playerId">プレイヤーID</param>
        /// <param name="deviceId">デバイスID</param>
        /// <param name="playerUid">プレイヤーUID</param>
        /// <param name="current">現在時刻</param>
        public async Task LoginAsync(long playerId, string deviceId, string playerUid, DateTimeOffset current)
        {
            // セッションID更新
            var session = new Session() { Id = PlayerAuth.GenerateSessionId(), LastLogin = current };
            await this.sessionRepository.Set(playerId, session);

            await playerAuthenticationRepository.SetPlayerIdAndSessionIdAsync(playerId, session.Id);

            // ログイン時間の更新
            var playerBasic = await playerBasicRepository.FindAsync(playerId);
            playerBasic.LastLogin = current;
            await playerBasicRepository.UpdateAsync(playerBasic);
        }
    }
}

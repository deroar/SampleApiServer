using SampleApiServer.Infra.Repositories;
using SampleApiServer.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SampleApiServer.Services
{
    /// <summary>
    /// アカウント登録サービス
    /// </summary>
    public class RegistrationService
    {
        private readonly IPlayerIdRepository playerIdRepository;
        private readonly IPlayerBoundRepository<PlayerAuth> playerAuthRepository;
        private readonly IPlayerBoundRepository<PlayerBasic> playerBasicRepository;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public RegistrationService(
            IPlayerIdRepository playerIdRepository,
            IPlayerBoundRepository<PlayerAuth> playerAuthRepository,
            IPlayerBoundRepository<PlayerBasic> playerBasicRepository)
        {
            this.playerIdRepository = playerIdRepository;
            this.playerAuthRepository = playerAuthRepository;
            this.playerBasicRepository = playerBasicRepository;
        }

        /// <summary>
        /// アカウント登録
        /// </summary>
        /// <param name="deviceId">デバイスID</param>
        /// <param name="playerUid">プレイヤーユニークID</param>
        /// <param name="name">プレイヤー名</param>
        /// <param name="dateTimeOffset">現在時間</param>
        /// <returns>プレイヤーID</returns>
        public async Task<long> RegisterAsync(string deviceId, string playerUid, string name, DateTimeOffset dateTimeOffset)
        {
            long playerId = await playerIdRepository.GetPlayerId();

            // 認証情報作成
            var playerAuth = PlayerAuth.Create(playerId, deviceId, playerUid);
            await playerAuthRepository.CreateAsync(playerAuth);

            var playerBasic = PlayerBasic.Create(playerId, name, dateTimeOffset);
            await playerBasicRepository.CreateAsync(playerBasic);

            return playerId;
        }
    }
}

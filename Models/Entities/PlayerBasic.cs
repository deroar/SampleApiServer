using System;

namespace SampleApiServer.Models.Entities
{
    public class PlayerBasic : PlayerBoundEntityBase
    {
        /// <summary>
        /// プレイヤー名
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// チュートリアル進捗度
        /// </summary>
        public int TutorialProgress { get; private set; }

        /// <summary>
        /// スタミナ値
        /// </summary>
        public int Stamina { get; private set; }

        /// <summary>
        /// 最終スタミナ回復時間
        /// </summary>
        public DateTimeOffset LastStaminaUpdatedAt { get; private set; } = default;

        /// <summary>
        /// 最終ログイン時間
        /// </summary>
        public DateTimeOffset  LastLogin { get; set; }

        /// <summary>
        /// PlayerBasicのエンティティを作成する
        /// </summary>
        /// <param name="playerId">プレイヤーID</param>
        /// <param name="name">プレイヤー名</param>
        /// <param name="dateTimeOffset">現在時間</param>
        /// <returns></returns>
        public static PlayerBasic Create(long playerId, string name, DateTimeOffset dateTimeOffset)
        {
            return new PlayerBasic
            {
                PlayerId = playerId,
                Name = name,
                TutorialProgress = 0,
                Stamina = 10,
                LastStaminaUpdatedAt = dateTimeOffset,
                LastLogin = default,
            };
        }
    }
}

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
        public DateTimeOffset  LastLogin { get; private set; }
    }
}

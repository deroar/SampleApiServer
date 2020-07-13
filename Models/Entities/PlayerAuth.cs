using SampleApiServer.Models.Entities;
using SampleApiServer.Util;
using System;
using System.ComponentModel.DataAnnotations;

namespace SampleApiServer.Models.Entities
{
    /// <summary>
    /// Playerの認証を表すクラス
    /// </summary>
    public class PlayerAuth : PlayerBoundEntityBase
    {
        /// <summary>
        /// hashとsaltの境の文字
        /// </summary>
        private const char HashSaltDelim = ':';

        /// <summary>
        /// デバイスID
        /// </summary>
        [MaxLength(32)]
        public string DeviceId { get; private set; }

        /// <summary>
        /// Hash化されたプレイヤーUID
        /// </summary>
        public string PlayerUidHash { get; private set; }

        /// <summary>
        /// PlayerAuthを作成する
        /// </summary>
        /// <param name="playerId">PlayerId</param>
        /// <param name="deviceId">デバイスID</param>
        /// <param name="playerUid">プレイヤーUID</param>
        /// <returns>PlayerAuth</returns>
        public static PlayerAuth Create(long playerId, string deviceId, string playerUid)
        {
            var (hash, salt) = Crypto.Generate(playerUid);
            string hashAndSalt = string.Join(HashSaltDelim, hash, salt);

            var entity = new PlayerAuth()
            {
                PlayerId = playerId,
                DeviceId = deviceId,
                PlayerUidHash = hashAndSalt,
            };
            return entity;
        }

        /// <summary>
        /// セッションID生成
        /// </summary>
        /// <returns></returns>
        public static string GenerateSessionId()
        {
            return Guid.NewGuid().ToString();
        }

        /// <summary>
        /// プレイヤー検証
        /// </summary>
        /// <param name="playerUid">プレイヤーUID</param>
        /// <returns>true:承認</returns>
        public bool Verify(string playerUid)
        {
            var hashes = PlayerUidHash.Split(HashSaltDelim);
            var hash = hashes[0];
            var salt = hashes[1];
            return Crypto.Verify(playerUid, hash, salt);
        }
    }
}

using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using SampleApiServer.Infra.Models;
using SampleApiServer.Models;
using System;
using System.Threading.Tasks;

namespace SampleApiServer.Infra.Repositories
{
    /// <summary>
    /// セッション情報を保持するリポジトリ。
    /// </summary>
    public class SessionRepository : RedisOperationRepositoryBase, ISessionRepository
    {
        #region 定数

        /// <summary>
        /// セッション情報を保持するRedisのキー名。
        /// </summary>
        private const string SessionKey = "session_data";

        /// <summary>
        /// セッション情報の最大保持期間 (秒)。
        /// </summary>
        /// <remarks>1日1回ログイン必須なので最大でも24時間。</remarks>
        private const int MaxExpiration = 86400;

        #endregion

        #region コンストラクタ

        /// <summary>
        /// 引き数をDIしてリポジトリを生成する。
        /// </summary>
        /// <param name="redis">Redisの接続オプション。</param>
        /// <param name="env">ホスティング情報。</param>
        public SessionRepository(EphemeralRedis redis, IHostEnvironment env)
            : base(SessionKey, env.EnvironmentName, redis.Connection)
        {
        }

        #endregion

        #region メソッド

        /// <summary>
        /// プレイヤーのセッション情報を取得する。
        /// </summary>
        /// <param name="playerId">プレイヤーID。</param>
        /// <returns>セッション情報。取得できない場合null。</returns>
        public async Task<Session> Get(long playerId)
        {
            var json = await this.Get(playerId.ToString());
            return string.IsNullOrEmpty(json) ? null : JsonConvert.DeserializeObject<Session>(json);
        }

        /// <summary>
        /// プレイヤーのセッション情報を登録する。
        /// </summary>
        /// <param name="playerId">プレイヤーID。</param>
        /// <param name="session">セッション情報。</param>
        /// <returns>処理状態。</returns>
        public async Task Set(long playerId, Session session)
        {
            await this.Set(playerId.ToString(), JsonConvert.SerializeObject(session), new TimeSpan(0, 0, MaxExpiration));
        }

        #endregion
    }
}

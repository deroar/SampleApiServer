using Microsoft.Extensions.Hosting;
using SampleApiServer.Infra.Models;
using System.Threading.Tasks;

namespace SampleApiServer.Infra.Repositories
{
    /// <summary>
    /// PlayerIDを扱うリポジトリ
    /// </summary>
    public class PlayerIdRepository : RedisOperationRepositoryBase, IPlayerIdRepository
    {
        private const string PlayerIdKey = "player_id";

        /// <summary>
        /// 引き数をDIしてリポジトリを生成する。
        /// </summary>
        /// <param name="redis">Redis接続オブジェクト。</param>
        /// <param name="env">ホスティング情報。</param>
        public PlayerIdRepository(PersistentRedis redis, IHostEnvironment env)
            : base(PlayerIdKey, env.EnvironmentName, redis.Connection)
        {
        }

        /// <summary>
        ///PlayerIDを取得する
        /// </summary>
        /// <returns>PlayerID</returns>
        public Task<long> GetPlayerId()
        {
            return Incr(PlayerIdKey);
        }
    }
}

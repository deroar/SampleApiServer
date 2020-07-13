using StackExchange.Redis;

namespace SampleApiServer.Infra.Models
{
    /// <summary>
    /// Redisコネクションを保持するクラス
    /// </summary>
    public abstract class RedisConnectionHolderBase
    {
        /// <summary>
        /// 接続オブジェクト
        /// </summary>
        public IConnectionMultiplexer Connection { get; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="conf">Redis接続設定</param>
        public RedisConnectionHolderBase(RedisConfig conf)
        {
            Connection = ConnectionMultiplexer.Connect(conf.RedisConnectionString);
        }
    }
}

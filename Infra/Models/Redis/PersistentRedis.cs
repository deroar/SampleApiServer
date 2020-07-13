namespace SampleApiServer.Infra.Models
{
    /// <summary>
    /// 永続化する（Persistentクラスタの）Redisコネクション
    /// </summary>
    public class PersistentRedis : RedisConnectionHolderBase
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="conf">Redis接続設定</param>
        public PersistentRedis(RedisConfig conf) : base(conf)
        {
        }
    }
}

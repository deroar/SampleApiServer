using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SampleApiServer.Infra.Models
{
    /// <summary>
    /// 揮発性の（Ephemeralクラスタの）Redisコネクションを保持するクラス
    /// </summary>
    public class EphemeralRedis : RedisConnectionHolderBase
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="conf">Redis接続設定</param>
        public EphemeralRedis(RedisConfig conf) : base(conf)
        {
        }
    }
}

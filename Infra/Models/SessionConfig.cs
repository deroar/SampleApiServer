using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SampleApiServer.Infra.Models
{
    /// <summary>
    /// セッション設定
    /// </summary>
    public class SessionConfig
    {
        /// <summary>
        /// Redisサーバ
        /// </summary>
        public string RedisServer { get; set; }

        /// <summary>
        /// RedisのデフォルトDB
        /// </summary>
        public string DefaultDatabase { get; set; }

        /// <summary>
        /// Redis接続情報
        /// </summary>
        public string Redis => $"{RedisServer},defaultDatabase={DefaultDatabase}";

        /// <summary>
        /// 有効期限
        /// </summary>
        public int ExpirationSec { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SampleApiServer.Infra.Models
{
    /// <summary>
    /// redisの接続先設定
    /// </summary>
    public class RedisConfig
    {
        // Redisのデフォルトポート
        private const int DefaultPort = 6379;

        /// <summary>
        /// 接続先一覧
        /// </summary>
        public string[] Hosts { get; set; }

        /// <summary>
        /// 接続するデータベース
        /// </summary>
        public int Db { get; set; }

        /// <summary>
        /// Redisの接続先文字列
        /// </summary>
        public string RedisConnectionString
        {
            get
            {
                var hostsWithPort = Hosts.Select(host => host.Contains(':') ? host : $"{host}:{DefaultPort}");
                return $"{string.Join(",", hostsWithPort)},defaultDatabase={Db}";
            }
        }
    }
}

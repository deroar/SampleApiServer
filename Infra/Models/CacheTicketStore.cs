using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using StackExchange.Redis;
using System;
using System.Threading.Tasks;

namespace SampleApiServer.Infra.Models
{
    /// <summary>
    /// Cookie認証用Redisセッションデータストア。
    /// </summary>
    /// <seealso href="https://qiita.com/tricrow/items/d171412405a3db918e1a"/>
    public class CacheTicketStore : ITicketStore
    {
        #region 定数

        /// <summary>
        /// Redisキーの接頭辞。
        /// </summary>
        private const string KeyPrefix = "AuthSessionStore-";

        #endregion

        #region メンバー変数

        /// <summary>
        /// セッションデータを保存するRedis DB。
        /// </summary>
        private readonly IDatabase cache;

        #endregion

        #region コンストラクタ

        /// <summary>
        /// 指定されたRedis DBを使用するデータストアを生成する。
        /// </summary>
        /// <param name="redisdb">接続先Redis DB。</param>
        public CacheTicketStore(IDatabase redisdb)
        {
            this.cache = redisdb;
        }

        #endregion

        #region インタフェースメソッド

        /// <summary>
        /// キーに紐づくデータを削除する。
        /// </summary>
        /// <param name="key">キー。</param>
        /// <returns>処理状態。</returns>
        public Task RemoveAsync(string key)
        {
            return this.cache.KeyDeleteAsync(key);
        }

        /// <summary>
        /// キーに紐づくデータを更新する。
        /// </summary>
        /// <param name="key">キー。</param>
        /// <param name="ticket">データ。</param>
        /// <returns>処理状態。</returns>
        public async Task RenewAsync(string key, AuthenticationTicket ticket)
        {
            byte[] val = SerializeToBytes(ticket);
            var expireUtc = ticket.Properties.ExpiresUtc;
            if (expireUtc.HasValue)
            {
                await this.cache.StringSetAsync(key, val, expireUtc.Value - DateTimeOffset.Now);
            }
            else
            {
                await this.cache.StringSetAsync(key, val);
            }
        }

        /// <summary>
        /// キーに紐づくデータを取得する。
        /// </summary>
        /// <param name="key">キー。</param>
        /// <returns>キーに紐づくデータ。</returns>
        public async Task<AuthenticationTicket> RetrieveAsync(string key)
        {
            var bytes = await this.cache.StringGetAsync(key, CommandFlags.PreferReplica);
            return DeserializeFromBytes(bytes);
        }

        /// <summary>
        /// データを登録して生成したキーに紐づける。
        /// </summary>
        /// <param name="ticket">データ。</param>
        /// <returns>キー。</returns>
        public async Task<string> StoreAsync(AuthenticationTicket ticket)
        {
            var guid = Guid.NewGuid();
            var key = KeyPrefix + guid.ToString();
            await this.RenewAsync(key, ticket);
            return key;
        }

        #endregion

        #region 内部メソッド

        /// <summary>
        /// チケットをシリアライズする。
        /// </summary>
        /// <param name="source">シリアライズするチケット。</param>
        /// <returns>シリアライズしたバイト列。</returns>
        private static byte[] SerializeToBytes(AuthenticationTicket source)
        {
            return TicketSerializer.Default.Serialize(source);
        }

        /// <summary>
        /// シリアライズされたチケットを復元する。
        /// </summary>
        /// <param name="source">シリアライズされたバイト列。</param>
        /// <returns>デシリアライズしたチケット。</returns>
        private static AuthenticationTicket DeserializeFromBytes(byte[] source)
        {
            return source == null ? null : TicketSerializer.Default.Deserialize(source);
        }

        #endregion
    }
}

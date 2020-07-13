using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SampleApiServer.Infra.Repositories
{
    /// <summary>
    /// Redisの特定のキーを操作するリポジトリを実装するためのベースクラス。
    /// </summary>
    /// <remarks>
    /// 一つのデータのみを扱うRedisリポジトリを実装する場合、このクラスを継承する。
    /// </remarks>
    public abstract class RedisOperationRepositoryBase : RedisRepositoryBase
    {
        #region メンバー変数

        /// <summary>
        /// 更新系操作が行われたか？
        /// </summary>
        private bool isUpdated = false;

        #endregion

        #region コンストラクタ

        /// <summary>
        /// 指定されたRedisコネクションを使用するリポジトリを生成する。
        /// </summary>
        /// <param name="dataName">データ名。</param>
        /// <param name="envName">ホスティング環境の名前情報。</param>
        /// <param name="redis">Redisコネクション。</param>
        public RedisOperationRepositoryBase(string dataName, string envName, IConnectionMultiplexer redis)
            : base(dataName, envName, redis)
        {
        }

        #endregion

        #region プロパティ

        /// <summary>
        /// Redis操作用インスタンス。
        /// </summary>
        protected IDatabase Db => this.redis.GetDatabase();

        #endregion

        #region Common Commands

        /// <summary>
        /// キーを削除する
        /// </summary>
        /// <param name="key">キー</param>
        /// <returns>処理状態。</returns>
        protected Task DeleteKey(string key)
        {
            this.TouchUpdated();
            return Db.KeyDeleteAsync(MakeKey(key));
        }

        /// <summary>
        /// キーの期限を取得する
        /// </summary>
        /// <param name="key">キー</param>
        /// <returns>期限。</returns>
        protected Task<TimeSpan?> GetTimeToLive(string key)
        {
            return Db.KeyTimeToLiveAsync(MakeKey(key));
        }

        /// <summary>
        /// キーの有効期限を設定
        /// </summary>
        /// <param name="key">リストのキー</param>
        /// <param name="limmitHours">制限時間</param>
        /// <returns>処理状態。</returns>
        protected async Task ExpireKey(string key, int limmitHours)
        {
            await Db.KeyExpireAsync(MakeKey(key), TimeSpan.FromHours(limmitHours));
        }

        #endregion

        #region String Commnands

        /// <summary>
        /// Strings型でkeyに対応するvalueを取得する
        /// </summary>
        /// <param name="key">取得するStrings型のキー</param>
        /// <returns>取得したvalueの文字列</returns>
        protected async Task<string> Get(string key)
        {
            return await this.Db.StringGetAsync(this.MakeKey(key), this.GetDefaultCommandFlagsForRead());
        }

        /// <summary>
        /// Strings型のkeyを指定してvalueをセットする
        /// </summary>
        /// <param name="key">セットするStrings型のキー</param>
        /// <param name="value">セットするStrings型のバリュー</param>
        /// <param name="expiration">有効期間</param>
        /// <returns>セットの成功可否をboolで返す。true:成功, false:失敗</returns>
        protected Task<bool> Set(string key, string value, TimeSpan? expiration = null)
        {
            this.TouchUpdated();
            return Db.StringSetAsync(MakeKey(key), value, expiration);
        }

        /// <summary>
        /// Strings型のkeyを指定してvalue分の値を増加させる
        /// </summary>
        /// <param name="key">増加させるキー</param>
        /// <param name="value">増加させる値</param>
        /// <param name="expiration">有効期間</param>
        /// <returns>増加せた分の結果</returns>
        protected async Task<long> Incr(string key, long value = 1, TimeSpan? expiration = null)
        {
            this.TouchUpdated();
            var result = await Db.StringIncrementAsync(MakeKey(key), value);
            await Db.KeyExpireAsync(MakeKey(key), expiration);
            return result;
        }

        /// <summary>
        /// 指定したキーが存在しない場合は、valueをセットする
        /// </summary>
        /// <param name="key">セットするStrings型のキー</param>
        /// <param name="value">セットするStrings型のバリュー</param>
        /// <param name="expiration">有効期間</param>
        /// <returns>セットの成功可否をboolで返す。true:成功, false:失敗</returns>
        protected Task<bool> Setnx(string key, string value, TimeSpan expiration)
        {
            this.TouchUpdated();
            return Db.StringSetAsync(MakeKey(key), value, expiration, When.NotExists);
        }

        #endregion

        #region Set Commands
        /// <summary>
        /// Setにmemberを追加する
        /// </summary>
        /// <param name="key">追加するSet型のキー</param>
        /// <param name="member">追加する値</param>
        /// <param name="expiration">有効期間</param>
        /// <returns>true : 新規に追加した / false : すでにメンバに含まれていた</returns>
        protected async Task<bool> SAdd(string key, string member, TimeSpan? expiration = null)
        {
            this.TouchUpdated();
            var result = await Db.SetAddAsync(MakeKey(key), member);
            await Db.KeyExpireAsync(MakeKey(key), expiration);
            return result;
        }

        /// <summary>
        /// Setからmemberを削除する
        /// </summary>
        /// <param name="key">削除するSet型のキー</param>
        /// <param name="member">削除する値</param>
        /// <returns>true : 新規に追加した / false : すでにメンバに含まれていた</returns>
        protected async Task<bool> SRem(string key, string member)
        {
            this.TouchUpdated();
            return await Db.SetRemoveAsync(MakeKey(key), member);
        }

        /// <summary>
        /// Setからすべてのmemberを得る。
        /// </summary>
        /// <param name="key">取得するSet型のキー。</param>
        /// <returns>メンバのリスト。</returns>
        protected async Task<IEnumerable<string>> SMembers(string key)
        {
            RedisValue[] results = await this.Db.SetMembersAsync(this.MakeKey(key), this.GetDefaultCommandFlagsForRead());
            return results.Select(r => (string)r);
        }

        #endregion


        #region 内部メソッド

        /// <summary>
        /// 更新実施を記録する。
        /// </summary>
        protected void TouchUpdated()
        {
            this.isUpdated = true;
        }

        /// <summary>
        /// このリポジトリでの参照系処理のデフォルトの<see cref="CommandFlags"/>を取得する。
        /// </summary>
        /// <returns>デフォルトのコマンドフラグ。</returns>
        /// <remarks>このクラスの実装では、通常はスレーブを、更新処理後はマスタを優先する。</remarks>
        protected CommandFlags GetDefaultCommandFlagsForRead()
        {
            // 更新処理の後はマスターを優先
            return this.isUpdated ? CommandFlags.PreferMaster : CommandFlags.PreferReplica;
        }

        #endregion
    }
}

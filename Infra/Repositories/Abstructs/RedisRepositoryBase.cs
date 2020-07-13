using StackExchange.Redis;

namespace SampleApiServer.Infra.Repositories
{
    /// <summary>
    /// Redis操作全般のリポジトリ実装用のベースクラス。
    /// </summary>
    public abstract class RedisRepositoryBase
    {
        #region 定数

        /// <summary>
        /// Redisキーのセパレータ。
        /// </summary>
        protected const char Separator = ':';

        #endregion

        #region メンバー変数

        /// <summary>
        /// Redisコネクション。
        /// </summary>
        protected readonly IConnectionMultiplexer redis;

        /// <summary>
        /// 環境名。
        /// </summary>
        private readonly string envName;

        /// <summary>
        /// データ名。
        /// </summary>
        private readonly string dataName;

        #endregion

        #region コンストラクタ

        /// <summary>
        /// 指定されたRedisコネクションを使用するリポジトリを生成する。
        /// </summary>
        /// <param name="dataName">データ名。</param>
        /// <param name="envName">ホスティング環境の名前情報。</param>
        /// <param name="redis">Redisコネクション。</param>
        public RedisRepositoryBase(string dataName, string envName, IConnectionMultiplexer redis)
        {
            this.redis = redis;
            this.envName = envName;
            this.dataName = dataName;
        }

        #endregion

        #region メソッド

        /// <summary>
        /// Redisキー用の文字列を生成する。
        /// </summary>
        /// <param name="key">キーのベース文字列。</param>
        /// <returns>生成したキー文字列。</returns>
        /// <remarks>環境名・データ名を付加したリポジトリごとのキーを生成する。</remarks>
        public string MakeKey(string key)
        {
            return string.Join(Separator, envName, dataName, key).ToLower();
        }

        #endregion
    }
}

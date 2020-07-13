using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SampleApiServer.Infra.Models
{
    /// <summary>
    /// MySQLデータベースの接続設定。
    /// </summary>
    /// <remarks>
    /// AppSettingsの設定値をバインドする用に定義。
    /// 必要に応じて各接続先用のサブクラスを定義して用いる。
    /// </remarks>
    /// <see href="https://mysqlconnector.net/connection-options/"/>
    public class MySQLConnectionConfig : ICloneable
    {
        #region 設定値

        /// <summary>
        /// 接続先のサーバー。
        /// </summary>
        public string Server { get; set; }

        /// <summary>
        /// 接続先のデータベース。
        /// </summary>
        public string Database { get; set; }

        /// <summary>
        /// 接続するユーザー。
        /// </summary>
        public string User { get; set; }

        /// <summary>
        /// ユーザーのパスワード。
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// コネクションプール可否（デフォルト可）。
        /// </summary>
        public bool? Pooling { get; set; }

        /// <summary>
        /// コネクションプール最大値（デフォルト100）。
        /// </summary>
        public int? MaxPoolSize { get; set; }

        #endregion

        #region 導出値

        /// <summary>
        /// 接続文字列。
        /// </summary>
        public string ConnectionString
        {
            get
            {
                var s = $"server={this.Server};database={this.Database};user={this.User};password={this.Password}";
                if (this.Pooling.HasValue)
                {
                    s += ";pooling=" + this.Pooling.Value.ToString().ToLower();
                }

                if (this.MaxPoolSize.HasValue)
                {
                    s += $";maxpoolsize={this.MaxPoolSize}";
                }

                return s;
            }
        }

        #endregion

        #region メソッド

        /// <summary>
        /// 設定をクローンする。
        /// </summary>
        /// <returns>クローンした接続設定。</returns>
        public object Clone()
        {
            return MemberwiseClone();
        }

        /// <summary>
        /// 現在の設定に別の設定値をマージする。
        /// </summary>
        /// <param name="from">マージする設定。</param>
        /// <returns>マージしたインスタンス。</returns>
        /// <remarks>
        /// 値が設定されていないプロパティに渡された設定の値が登録されて返る。
        /// （マージ結果は戻り値のみ。現在のインスタンスは更新されない。）
        /// YAMLの機能でも出来る筈だが、YamlDotNetが未対応だったため自前で似た仕組みを実装。
        /// </remarks>
        public MySQLConnectionConfig Merge(MySQLConnectionConfig from)
        {
            var merged = this.Clone() as MySQLConnectionConfig;
            if (string.IsNullOrEmpty(this.Server))
            {
                merged.Server = from.Server;
            }

            if (string.IsNullOrEmpty(this.Database))
            {
                merged.Database = from.Database;
            }

            if (string.IsNullOrEmpty(this.User))
            {
                merged.User = from.User;
            }

            if (string.IsNullOrEmpty(this.Password))
            {
                merged.Password = from.Password;
            }

            if (this.Pooling == null)
            {
                merged.Pooling = from.Pooling;
            }

            if (this.MaxPoolSize == null)
            {
                merged.MaxPoolSize = from.MaxPoolSize;
            }

            return merged;
        }

        #endregion
    }

    /// <summary>
    /// データベースのデフォルト設定。
    /// </summary>
    /// <remarks>
    /// 同じような設定を大量に書くとミスしやすいので、
    /// その対処用のデフォルト設定として定義。マージして用いる。
    /// </remarks>
    public class DefaultMySQLConnectionConfig : MySQLConnectionConfig
    {
    }

    /// <summary>
    /// PlayerBoundDBContext用のデータベースの設定。
    /// </summary>
    public class PlayerBoundMySQLConnectionConfig : MySQLConnectionConfig
    {
    }
}

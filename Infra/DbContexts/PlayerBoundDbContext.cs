using Microsoft.EntityFrameworkCore;
using SampleApiServer.Models.Entities;

namespace SampleApiServer.Infra.DbContexts
{
    public class PlayerBoundDbContext : DbContext
    {

        #region Entities

        public DbSet<PlayerBasic> PlayerBasics { get; set; }

        #endregion

        #region メンバ変数

        /// <summary>
        /// Mysql接続文字列
        /// </summary>
        private readonly string ConnectionString;

        #endregion


        /// <summary>
        /// 指定されたMySQLに接続する。
        /// </summary>
        /// <param name="connectionString">MySQLの接続文字列。</param>
        public PlayerBoundDbContext(string connectionString)
        {
            this.ConnectionString = connectionString;
        }

        /// <summary>
        /// サブクラスでの拡張用のコンストラクタ。
        /// </summary>
        /// <param name="options">DBコンテキストのオプション。</param>
        protected PlayerBoundDbContext(DbContextOptions options) : base(options)
        {
        }

        #region メソッド

        /// <summary>
        /// DBコンテキストの設定処理。
        /// </summary>
        /// <param name="optionsBuilder">オプションのビルダー。</param>
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.UseMySql(ConnectionString);
        }

        /// <summary>
        /// OnModelCreatingのオーバーライド
        /// </summary>
        /// <param name="modelBuilder">modelBuilder</param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Primary Key
            modelBuilder.Entity<PlayerAuth>().HasKey(p => new { p.PlayerId, p.DeviceId });
            modelBuilder.Entity<PlayerBasic>().HasKey(p => p.PlayerId);

            // Index
            modelBuilder.Entity<PlayerBasic>().HasIndex(p => new { p.CreatedAt });
        }

        #endregion
    }
}

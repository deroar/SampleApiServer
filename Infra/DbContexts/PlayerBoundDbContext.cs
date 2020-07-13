using Microsoft.EntityFrameworkCore;
using SampleApiServer.Models.Entities;

namespace SampleApiServer.Infra.DbContexts
{
    public class PlayerBoundDbContext : DbContext
    {

        #region Entities

        public DbSet<PlayerAuth> PlayerAuths { get; set; }
        public DbSet<PlayerBasic> PlayerBasics { get; set; }

        #endregion

        /// <summary>
        /// サブクラスでの拡張用のコンストラクタ。
        /// </summary>
        /// <param name="options">DBコンテキストのオプション。</param>
        public PlayerBoundDbContext(DbContextOptions options) : base(options)
        {
        }

        #region メソッド

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

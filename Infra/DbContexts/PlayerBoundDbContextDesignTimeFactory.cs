using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using SampleApiServer.Infra.Models;
using SampleApiServer.Infra.Util.Extensions;

namespace SampleApiServer.Infra.DbContexts
{
    /// <summary>
    /// <see cref="PlayerBoundDbContext"/>のマイグレーション時に呼ばれるファクトリクラス。
    /// </summary>
    /// <remarks>
    /// </remarks>
    public class PlayerBoundDbContextDesignTimeFactory : IDesignTimeDbContextFactory<PlayerBoundDbContext>
    {
        /// <summary>
        /// <see cref="PlayerBoundDbContext"/>を生成する。
        /// </summary>
        /// <param name="args">未使用。</param>
        /// <returns>生成したDBコンテキスト。</returns>
        /// <remarks>
        /// </remarks>
        public PlayerBoundDbContext CreateDbContext(string[] args)
        {
            var configuration = new ConfigurationBuilder().ApplyMigrationConfig().Build();

            var defaultConf = configuration.GetSection("MySql:Default").Get<DefaultMySQLConnectionConfig>();
            var connectionConf = configuration.GetSection("MySql:Player").Get<PlayerBoundMySQLConnectionConfig>();

            var conf = connectionConf.Merge(defaultConf);
            return new PlayerBoundDbContext(conf.ConnectionString);
        }
    }
}

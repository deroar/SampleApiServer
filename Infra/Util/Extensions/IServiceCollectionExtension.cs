using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SampleApiServer.Infra.DbContexts;
using SampleApiServer.Infra.Models;
using SampleApiServer.Infra.Repositories;
using SampleApiServer.Infra.Transaction;
using SampleApiServer.Models.Entities;
using SampleApiServer.Services;
using System;
using System.Collections.Generic;

namespace SampleApiServer.Infra.Util.Extensions
{
    /// <summary>
    /// IServiceCollectionに依存性を登録する拡張
    /// </summary>
    public static class IServiceCollectionExtension
    {
        /// <summary>
        /// 依存性を登録する
        /// </summary>
        /// <param name="services">IServiceCollection</param>
        /// <param name="configuration">IConfiguration</param>
        public static IServiceCollection AddSeverServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddHttpClient();

            // 通常はRedisキャッシュを用いるが、それで足りない処理のためにインメモリキャッシュも有効化
            services.AddMemoryCache();

            services.AddEntityRepositories();
            services.AddDomainServices();

            var persistentRedisConf = configuration.GetSection("Redis:Persistent").Get<RedisConfig>();
            services.AddSingleton(new PersistentRedis(persistentRedisConf));
            var ephemeralRedisConf = configuration.GetSection("Redis:Ephemeral").Get<RedisConfig>();
            services.AddSingleton(new EphemeralRedis(ephemeralRedisConf));

            services.Configure<DefaultMySQLConnectionConfig>(configuration.GetSection("MySQL:Default"));
            services.Configure<PlayerBoundMySQLConnectionConfig>(configuration.GetSection("MySQL:Player"));

            services.AddScoped<ITransactionManager, TransactionManager>();
            services.AddScoped<IPlayerAuthenticationRepository, PlayerAuthenticationCookieRepository>();
            services.AddScoped<IPlayerIdRepository, PlayerIdRepository>();
            services.AddScoped<ISessionRepository, SessionRepository>();

            var defaultConf = configuration.GetSection("MySql:Default").Get<DefaultMySQLConnectionConfig>();
            var playerBoundConnectionConf = configuration.GetSection("MySql:Player").Get<PlayerBoundMySQLConnectionConfig>();
            var playerBoundconf = playerBoundConnectionConf.Merge(defaultConf);
            services.AddDbContextPool<PlayerBoundDbContext>(o => o.UseMySql(playerBoundconf.ConnectionString));

            return services;
        }

        /// <summary>
        /// HttpClient関連の依存性を登録する
        /// </summary>
        /// <param name="services">IServiceCollection</param>
        /// <param name="configuration">IConfiguration</param>
        /// <returns>IServiceCollection</returns>
        public static IServiceCollection AddHttpClient(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddHttpClient();
            return services;
        }

        /// <summary>
        /// DIコンテナにEntityを扱うリポジトリを追加する。
        /// </summary>
        /// <param name="services">サービスコレクション。</param>
        private static void AddEntityRepositories(this IServiceCollection services)
        {
            // ここに Entity の型を追加していく
            var entityTypes = new Type[]
            {
                // PlayerBound
                typeof(PlayerAuth),
                typeof(PlayerBasic),
            };

            foreach (Type entityType in entityTypes)
            {
                if (typeof(PlayerBoundEntityBase).IsAssignableFrom(entityType))
                {
                    // DB用のリポジトリを登録
                    Type iPlayerRepoType = typeof(IPlayerBoundRepository<>).MakeGenericType(entityType);
                    Type efPlayerRepoType = typeof(PlayerBoundRepository<>).MakeGenericType(entityType);
                    services.AddScoped(iPlayerRepoType, efPlayerRepoType);

                    // プレイヤーエンティティについては、参照頻度が高いためキャッシュも設定する
                    Type iCacheType = typeof(ICacheRepository<>).MakeGenericType(entityType);
                    Type redisCacheType = typeof(PlayerEntityCacheRepository<>).MakeGenericType(entityType);
                    services.AddScoped(iCacheType, redisCacheType);
                }
            }
        }

        // DI コンテナに Service を追加する
        private static void AddDomainServices(this IServiceCollection services)
        {
            // ここに Service を追加していく
            var serviceTypes = new Type[]
            {
                typeof(RegistrationService),
            };

            foreach (Type serviceType in serviceTypes)
            {
                services.AddScoped(serviceType);
            }
        }
    }
}

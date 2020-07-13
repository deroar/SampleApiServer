using Microsoft.Extensions.Configuration;
using System;
using System.IO;

namespace SampleApiServer.Infra.Util.Extensions
{
    /// <summary>
    /// <see cref="IConfigurationBuilder"/>にアプリ用の設定を適用する拡張。
    /// </summary>
    public static class IConfigurationBuilderExtension
    {
        /// <summary>
        /// 設定ビルダーにアプリ用の設定を適用する。
        /// </summary>
        /// <param name="config">設定を適用するビルダー。</param>
        /// <returns>設定を適用したビルダー。</returns>
        public static IConfigurationBuilder ApplyAppConfig(this IConfigurationBuilder config)
        {
            string envName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            string subEnvName = (Environment.GetEnvironmentVariable("ASPNETCORE_SUB_ENVIRONMENT") ?? "Local").ToLower();
            return config
                .AddYamlFile("Configs/default.yml", optional: false)
                .AddYamlFile($"Configs/{envName}/default.yml", optional: true)
                .AddYamlFile($"Configs/{envName}/{subEnvName}.yml", optional: true)
                .AddEnvironmentVariables(prefix: "SAMPLEAPI_");
        }

        /// <summary>
        /// 設定ビルダーにアプリのマイグレーション用の設定を適用する。
        /// </summary>
        /// <param name="config">設定を適用するビルダー。</param>
        /// <returns>設定を適用したビルダー。</returns>
        public static IConfigurationBuilder ApplyMigrationConfig(this IConfigurationBuilder config)
        {
            string envName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            string subEnvName = (Environment.GetEnvironmentVariable("ASPNETCORE_SUB_ENVIRONMENT") ?? "Local").ToLower();
            return config
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddYamlFile("Configs/default.yml", optional: false)
                .AddYamlFile($"Configs/{envName}/default.yml", optional: true)
                .AddYamlFile($"Configs/{envName}/{subEnvName}.yml", optional: true)
                .AddYamlFile($"Configs/{envName}/{subEnvName}.migration.yml", optional: true)
                .AddEnvironmentVariables(prefix: "SAPMLEAPI_");
        }
    }
}

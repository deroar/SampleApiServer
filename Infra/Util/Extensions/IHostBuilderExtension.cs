using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SampleApiServer.Infra.Util.Extensions
{
    /// <summary>
    /// <see cref="IHostBuilder"/>に設定読み込みセットを登録する拡張。
    /// </summary>
    public static class IHostBuilderExtension
    {
        /// <summary>
        /// 設定読み込みセットを登録する。
        /// </summary>
        /// <remarks>
        /// <see cref="IHostBuilder.ConfigureAppConfiguration"/>の設定読み込みセット。
        /// </remarks>
        public static IHostBuilder ConfigureAppConfiguration(this IHostBuilder hostBuilder)
        {
            return hostBuilder.ConfigureAppConfiguration((hostingContext, config) =>
            {
                config.ApplyAppConfig();
            });
        }
    }
}

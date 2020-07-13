using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using SampleApiServer.Infra.Util.Extensions;
using System.Threading.Tasks;

namespace SampleApiServer
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            await CreateHostBuilder(args).Build().RunAsync();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration()
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}

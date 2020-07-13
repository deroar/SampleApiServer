using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.DataProtection.StackExchangeRedis;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using SampleApiServer.Filters;
using SampleApiServer.Infra.Models;
using SampleApiServer.Infra.Util.Extensions;
using StackExchange.Redis;
using System;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using SampleApiServer.Util.Swagger;
using SampleApiServer.Middlewares;

namespace SampleApiServer
{
    /// <summary>
    /// Webアプリケーション初期設定用のクラスです。
    /// </summary>
    public class Startup
    {
        /// <summary>
        /// StartUpのコンストラクタ
        /// </summary>
        /// <param name="configuration">アプリケーション設定。</param>
        /// <param name="environment">ホスティング環境情報。</param>
        public Startup(IConfiguration configuration, IHostEnvironment environment)
        {
            Configuration = configuration;
            Environment = environment;
        }

        /// <summary>
        /// アプリケーション設定。
        /// </summary>
        public IConfiguration Configuration { get; }

        /// <summary>
        /// ホスティング環境情報。
        /// </summary>
        public IHostEnvironment Environment { get; }

        /// <summary>
        /// Webアプリケーションのサービス設定用メソッド。
        /// </summary>
        /// <param name="services">DIされるサービスのコンテナ。</param>
        /// <remarks>設定値の登録や依存関係の登録など、アプリ初期化前の設定を行う。</remarks>
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHealthChecks();

            // 認証用セッション設定。本番では複数インスタンスが稼働するのでRedisに保存する
            var sessionConfig = this.Configuration.GetSection("Session").Get<SessionConfig>();
            var sessionRedis = ConnectionMultiplexer.Connect(sessionConfig.Redis);
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.Cookie.Name = ".Session";
                    options.Cookie.HttpOnly = true;
                    options.Cookie.IsEssential = true;
                    options.Cookie.SameSite = SameSiteMode.Strict;
                    options.Events.OnRedirectToAccessDenied = ReplaceRedirector(HttpStatusCode.Forbidden);
                    options.Events.OnRedirectToLogin = ReplaceRedirector(HttpStatusCode.Unauthorized);
                    options.ExpireTimeSpan = TimeSpan.FromSeconds(sessionConfig.ExpirationSec);
                    options.SessionStore = new CacheTicketStore(sessionRedis.GetDatabase());
                });
            services.AddDataProtection()
                .SetApplicationName("sample-api")
                .PersistKeysToStackExchangeRedis(sessionRedis, "DataProtection-Keys");

            // コントローラとフィルタの登録
            services.AddControllers(option =>
            {
                option.Filters.Add(typeof(ApiExceptionFilter));
                option.Filters.Add(new ApiResponseFilter());
            })
                .AddNewtonsoftJson(opt => opt.SerializerSettings.NullValueHandling = NullValueHandling.Ignore)
                .UseErrorResponseOnModelValidation(Environment);
            services.AddHttpContextAccessor();

            // Swagger定義の設定
            services.AddSwaggerGen(c =>
            {
                var asm = Assembly.GetExecutingAssembly();
                var product = asm.GetCustomAttribute(typeof(AssemblyProductAttribute)) as AssemblyProductAttribute;
                c.SwaggerDoc("v1", new OpenApiInfo { Title = product.Product, Version = asm.GetName().Version.ToString() });
                c.IncludeXmlComments(AppDomain.CurrentDomain.BaseDirectory + $"{asm.GetName().Name}.xml");
                c.OperationFilter<AddHeaderOperationFilter>();
            });

            services.AddHttpClient(Configuration);
            services.AddNgeServices(Configuration);
            services.AddApplicationInsightsTelemetry();
        }

        internal static Func<RedirectContext<CookieAuthenticationOptions>, Task> ReplaceRedirector(HttpStatusCode statusCode)
        {
            return context =>
            {
                context.Response.StatusCode = (int)statusCode;
                return Task.CompletedTask;
            };
        }

        /// <summary>
        /// Webアプリケーションの設定用メソッド。
        /// このメソッド内でHTTPリクエストのパイプライン等を設定する。
        /// </summary>
        /// <param name="app">DIされるアプリケーションビルダ</param>
        /// <param name="env">DIされる環境情報</param>
        /// <param name="serviceProvider">DIされるサービスプロバイダ。</param>
        /// <remarks>初期化されたインスタンスなどを元に、アプリ起動前の設定を行う。</remarks>
        public void Configure(IApplicationBuilder app, IHostEnvironment env, IServiceProvider serviceProvider)
        {
            app.UseHealthChecks("/health");

            var localizationOption = app.ApplicationServices.GetService<IOptions<RequestLocalizationOptions>>();
            app.UseRequestLocalization(localizationOption.Value);

            if (env.IsDevelopment() || env.EnvironmentName == "Testing")
            {
                app.UseDeveloperExceptionPage();

                // Swagger JSONとUIのエンドポイントを有効化
                app.UseSwagger(c =>
                {
                    // パスを小文字化
                    c.PreSerializeFilters.Add((doc, _) =>
                    {
                        foreach (var key in doc.Paths.Keys.ToArray())
                        {
                            var v = doc.Paths[key];
                            doc.Paths.Remove(key);
                            doc.Paths[key.ToLowerExceptBetweenBraces()] = v;
                        }
                    });
                });
                app.UseSwaggerUI(c =>
                {
                    var asm = Assembly.GetExecutingAssembly();
                    var product = asm.GetCustomAttribute(typeof(AssemblyProductAttribute)) as AssemblyProductAttribute;
                    c.SwaggerEndpoint("v1/swagger.json", product.Product);
                    c.EnableDeepLinking();
                });
            }

            app.UseRouting();

            //app.UseMiddleware<EnableBufferingMiddleware>();
            //app.UseAccessLogMiddleware();
            app.UseMiddleware<ErrorHandlingMiddleware>();
            //app.UseAppVersionCheckMiddleware();

            app.UseAuthentication();
            app.UseAuthorization();


            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}

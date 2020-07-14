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
    /// Web�A�v���P�[�V���������ݒ�p�̃N���X�ł��B
    /// </summary>
    public class Startup
    {
        /// <summary>
        /// StartUp�̃R���X�g���N�^
        /// </summary>
        /// <param name="configuration">�A�v���P�[�V�����ݒ�B</param>
        /// <param name="environment">�z�X�e�B���O�����B</param>
        public Startup(IConfiguration configuration, IHostEnvironment environment)
        {
            Configuration = configuration;
            Environment = environment;
        }

        /// <summary>
        /// �A�v���P�[�V�����ݒ�B
        /// </summary>
        public IConfiguration Configuration { get; }

        /// <summary>
        /// �z�X�e�B���O�����B
        /// </summary>
        public IHostEnvironment Environment { get; }

        /// <summary>
        /// Web�A�v���P�[�V�����̃T�[�r�X�ݒ�p���\�b�h�B
        /// </summary>
        /// <param name="services">DI�����T�[�r�X�̃R���e�i�B</param>
        /// <remarks>�ݒ�l�̓o�^��ˑ��֌W�̓o�^�ȂǁA�A�v���������O�̐ݒ���s���B</remarks>
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHealthChecks();

            // �F�ؗp�Z�b�V�����ݒ�B�{�Ԃł͕����C���X�^���X���ғ�����̂�Redis�ɕۑ�����
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

            // �R���g���[���ƃt�B���^�̓o�^
            services.AddControllers(option =>
            {
                option.Filters.Add(typeof(ApiExceptionFilter));
                option.Filters.Add(new ApiResponseFilter());
            })
                .AddNewtonsoftJson(opt => opt.SerializerSettings.NullValueHandling = NullValueHandling.Ignore)
                .UseErrorResponseOnModelValidation(Environment);
            services.AddHttpContextAccessor();

            // Swagger��`�̐ݒ�
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
        /// Web�A�v���P�[�V�����̐ݒ�p���\�b�h�B
        /// ���̃��\�b�h����HTTP���N�G�X�g�̃p�C�v���C������ݒ肷��B
        /// </summary>
        /// <param name="app">DI�����A�v���P�[�V�����r���_</param>
        /// <param name="env">DI���������</param>
        /// <param name="serviceProvider">DI�����T�[�r�X�v���o�C�_�B</param>
        /// <remarks>���������ꂽ�C���X�^���X�Ȃǂ����ɁA�A�v���N���O�̐ݒ���s���B</remarks>
        public void Configure(IApplicationBuilder app, IHostEnvironment env, IServiceProvider serviceProvider)
        {
            app.UseHealthChecks("/health");

            var localizationOption = app.ApplicationServices.GetService<IOptions<RequestLocalizationOptions>>();
            app.UseRequestLocalization(localizationOption.Value);

            if (env.IsDevelopment() || env.EnvironmentName == "Testing")
            {
                app.UseDeveloperExceptionPage();

                // Swagger JSON��UI�̃G���h�|�C���g��L����
                app.UseSwagger(c =>
                {
                    // �p�X����������
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

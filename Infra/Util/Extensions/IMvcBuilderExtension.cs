using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SampleApiServer.Exceptions;
using SampleApiServer.Models;
using System.Linq;

namespace SampleApiServer.Infra.Util.Extensions
{
    /// <summary>
    /// <see cref="IMvcBuilder"/>に、モデルバリデーションエラー時に <see cref="ApiErrorResponse"/> を使う設定を追加する拡張
    /// </summary>
    public static class IMvcBuilderExtension
    {
        /// <summary>
        /// <see cref="IMvcBuilder"/>に、モデルバリデーションエラー時に <see cref="ApiErrorResponse"/> を使う設定を追加する
        /// </summary>
        /// <param name="mvcBuilder">対象の<see cref="IMvcBuilder"/></param>
        /// <param name="env">ホスティング環境情報</param>
        public static IMvcBuilder UseErrorResponseOnModelValidation(this IMvcBuilder mvcBuilder, IHostEnvironment env)
        {
            return mvcBuilder.ConfigureApiBehaviorOptions(option =>
            {
                option.InvalidModelStateResponseFactory = ctx =>
                {
                    if (env.IsDevelopment() || env.EnvironmentName == "Testing")
                    {
                        var errorMessages = ctx.ModelState.SelectMany(x => x.Value.Errors.Select(z => z.ErrorMessage));
                        var message = string.Join("\n", errorMessages);
                        return new BadRequestObjectResult(new ApiErrorDebugResponse
                        {
                            Error = (int)ErrorCode.BAD_REQUEST,
                            DebugMessage = message,
                        });
                    }
                    else
                    {
                        return new BadRequestObjectResult(new ApiErrorResponse
                        {
                            Error = (int)ErrorCode.BAD_REQUEST,
                        });
                    }
                };
            });
        }
    }
}

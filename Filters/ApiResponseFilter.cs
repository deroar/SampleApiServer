using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using SampleApiServer.Models;
using SampleApiServer.Util;
using System;
using System.Threading.Tasks;

namespace SampleApiServer.Filters
{
    /// <summary>
    /// 正常に終了したAPIをハンドリングするFilter
    /// </summary>
    public class ApiResponseFilter : IAsyncActionFilter
    {
        /// <summary>
        /// アクション実行時のhook
        /// </summary>
        /// <param name="context">アクション実行前のコンテキスト</param>
        /// <param name="next">次のアクション</param>
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var afterContext = await next();

            if (IsSwagger(afterContext.HttpContext))
            {
                return;
            }
            var localOffset = DateTimeOffset.Now;

            // TODO: Hash(nonce)はいったん空設定
            var hash = string.Empty;

            if (afterContext.Result is ObjectResult objectResult)
            {
                var apiResponse = new ApiResponse(localOffset.ToUnixTimeSeconds(), hash, objectResult.Value);
                var commonResult = new ObjectResult(apiResponse)
                {
                    Formatters = objectResult.Formatters,
                    ContentTypes = objectResult.ContentTypes,
                    DeclaredType = apiResponse.GetType(),
                    StatusCode = objectResult.StatusCode
                };
                afterContext.Result = commonResult;
            }
            else if (afterContext.Result is EmptyResult emptyResult)
            {
                var apiResponse = new ApiResponse(localOffset.ToUnixTimeSeconds(), hash, null);
                var commonResult = new ObjectResult(apiResponse)
                {
                    DeclaredType = apiResponse.GetType(),
                };
                afterContext.Result = commonResult;
            }
        }

        private bool IsSwagger(HttpContext context)
        {
            return context.Request.Path.StartsWithSegments("/swagger");
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.Hosting;
using SampleApiServer.Exceptions;
using SampleApiServer.Models;
using System.Net;

namespace SampleApiServer.Filters
{
    /// <summary>
    /// 例外をハンドリングするFilter
    /// </summary>
    public class ApiExceptionFilter : IExceptionFilter
    {
        private readonly IHostEnvironment hostEnvironment;

        /// <summary>
        /// 引き数をDIしてフィルターを生成する。
        /// </summary>
        /// <param name="hostEnvironment">環境情報。</param>
        public ApiExceptionFilter(IHostEnvironment hostEnvironment)
        {
            this.hostEnvironment = hostEnvironment;
        }

        /// <summary>
        /// 例外発生時のハンドリング
        /// </summary>
        /// <param name="context">例外を含むコンテキスト</param>
        public void OnException(ExceptionContext context)
        {
            HttpStatusCode statusCode = HttpStatusCode.InternalServerError;
            ErrorCode error = ErrorCode.UNKNOWN;
            object result;

            if (context.Exception is ApiException exception)
            {
                error = exception.ErrorCode;
                statusCode = exception.StatusCode;
            }

            // 開発環境はデバッグ用メッセージを出す
            if (this.hostEnvironment.IsDevelopment())
            {
                result = new ApiErrorDebugResponse()
                {
                    Error = (int)error,
                    DebugMessage = context.Exception.ToString()
                };
            }
            else
            {
                result = new ApiErrorResponse()
                {
                    Error = (int)error
                };
            }

            context.HttpContext.Response.StatusCode = (int)statusCode;

            var contextTypes = new MediaTypeCollection();
            contextTypes.Add("application/json");
            context.Result = new ObjectResult(result)
            {
                ContentTypes = contextTypes,
                DeclaredType = result.GetType()
            };
        }
    }
}

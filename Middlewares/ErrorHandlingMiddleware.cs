using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using SampleApiServer.Exceptions;
using SampleApiServer.Models;
using System;
using System.Net;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using System.Threading.Tasks;

namespace SampleApiServer.Middlewares
{
    /// <summary>
    /// エラー処理ミドルウェアクラス。
    /// </summary>
    public class ErrorHandlingMiddleware
    {
        #region メンバー変数

        /// <summary>
        /// 次の処理のデリゲート。
        /// </summary>
        private readonly RequestDelegate next;

        /// <summary>
        /// 環境情報。
        /// </summary>
        private readonly IHostEnvironment env;

        #endregion

        #region コンストラクタ

        /// <summary>
        /// ミドルウェアを生成する。
        /// </summary>
        /// <param name="next">次の処理のデリゲート。</param>
        /// <param name="env">環境情報。</param>
        public ErrorHandlingMiddleware(RequestDelegate next, IHostEnvironment env)
        {
            this.next = next;
            this.env = env;
        }

        #endregion

        #region ミドルウェアメソッド

        /// <summary>
        /// ミドルウェアを実行する。
        /// </summary>
        /// <param name="context">HTTPコンテキスト。</param>
        /// <returns>処理状態。</returns>
        public async Task Invoke(HttpContext context)
        {
            // ※ 現状、例外以外のエラー（直接404を返す等）は処理出来ていないので注意
            try
            {
                await this.next(context);
            }
            catch (Exception ex)
            {
                await this.OnException(context, ex);
            }
        }

        #endregion

        #region 内部メソッド

        /// <summary>
        /// 例外を処理する。
        /// </summary>
        /// <param name="context">HTTPコンテキスト。</param>
        /// <param name="exception">発生した例外。</param>
        /// <returns>処理状態。</returns>
        private Task OnException(HttpContext context, Exception exception)
        {
            // 例外を元にエラー情報を返す
            var err = this.MakeErrorResponse(exception);

            // HTTPステータスコードを判定
            var status = HttpStatusCode.InternalServerError;
            if (exception is ApiException ngeEx)
            {
                status = ngeEx.StatusCode;
            }

            // エラーログを出力。ステータスコードに応じてログレベルを切り替え。
            if ((int)status >= 500)
            {
                // TODO: エラーログ出力
            }
            else
            {
                // TODO: ログ出力
            }

            // レスポンスを生成
            var result = JsonSerializer.Serialize(
                err,
                new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
                });

            context.Response.Clear();
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)status;
            return context.Response.WriteAsync(result);
        }

        /// <summary>
        /// エラーレスポンスを生成する。
        /// </summary>
        /// <param name="e">発生した例外。</param>
        /// <returns>エラーレスポンス。</returns>
        private object MakeErrorResponse(Exception e)
        {
            var error = ErrorCode.UNKNOWN;
            if (e is ApiException ngeEx)
            {
                error = ngeEx.ErrorCode;
            }

            if (this.env.IsDevelopment() || this.env.EnvironmentName == "Testing")
            {
                return new ApiErrorDebugResponse()
                {
                    Error = (int)error,
                    DebugMessage = e.ToString()
                };
            }
            else
            {
                return new ApiErrorResponse()
                {
                    Error = (int)error
                };
            }
        }

        #endregion
    }
}

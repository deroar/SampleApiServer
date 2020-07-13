using System;
using System.Net;

namespace SampleApiServer.Exceptions
{
    /// <summary>
    /// サーバーエラー用の例外クラス。
    /// </summary>
    /// <remarks>HTTPレスポンスの場合、<see href="https://developer.mozilla.org/ja/docs/Web/HTTP/Status/500">500 Internal Server Error</see>を返す。</remarks>
    public class ServerException : ApiException
    {
        /// <summary>
        /// 渡されたエラーコードでサーバーエラーの例外を生成する。
        /// </summary>
        /// <param name="errorCode">エラーコード。</param>
        public ServerException(ErrorCode errorCode) : base(errorCode, HttpStatusCode.InternalServerError)
        {
        }

        /// <summary>
        /// 渡されたエラーコードとメッセージでサーバーエラーの例外を生成する。
        /// </summary>
        /// <param name="errorCode">エラーコード。</param>
        /// <param name="message">エラーメッセージ。</param>
        public ServerException(ErrorCode errorCode, string message) : base(errorCode, HttpStatusCode.InternalServerError, message)
        {
        }

        /// <summary>
        /// 渡されたエラーコードとメッセージと例外でサーバーエラーの例外を生成する。
        /// </summary>
        /// <param name="errorCode">エラーコード。</param>
        /// <param name="message">エラーメッセージ。</param>
        /// <param name="innerException">元になった例外。</param>
        public ServerException(ErrorCode errorCode, string message, Exception innerException) : base(errorCode, HttpStatusCode.InternalServerError, message, innerException)
        {
        }
    }
}

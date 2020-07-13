using System;
using System.Net;

namespace SampleApiServer.Exceptions
{
    /// <summary>
    /// 例外の元となる抽象クラス。
    /// </summary>
    public abstract class ApiException : Exception
    {
        #region コンストラクタ

        /// <summary>
        /// 渡されたエラーコードとHTTPステータスコードで例外を生成する。
        /// </summary>
        /// <param name="errorCode">エラーコード。</param>
        /// <param name="statusCode">HTTPステータスコード。</param>
        protected ApiException(ErrorCode errorCode, HttpStatusCode statusCode) : this(errorCode, statusCode, null)
        {
        }

        /// <summary>
        /// 渡されたエラーコードとHTTPステータスコードとメッセージで例外を生成する。
        /// </summary>
        /// <param name="errorCode">エラーコード。</param>
        /// <param name="statusCode">HTTPステータスコード。</param>
        /// <param name="message">エラーメッセージ。</param>
        protected ApiException(ErrorCode errorCode, HttpStatusCode statusCode, string message) : this(errorCode, statusCode, message, null)
        {
        }

        /// <summary>
        /// 渡されたエラーコードとHTTPステータスコードとメッセージと例外で例外を生成する。
        /// </summary>
        /// <param name="errorCode">エラーコード。</param>
        /// <param name="statusCode">HTTPステータスコード。</param>
        /// <param name="message">エラーメッセージ。</param>
        /// <param name="innerException">元になった例外。</param>
        protected ApiException(ErrorCode errorCode, HttpStatusCode statusCode, string message, Exception innerException)
            : base(message ?? errorCode.ToString(), innerException)
        {
            this.ErrorCode = errorCode;
            this.StatusCode = statusCode;
        }

        #endregion

        #region プロパティ

        /// <summary>
        /// クライアントに返却するエラーコード。
        /// </summary>
        public ErrorCode ErrorCode { get; }

        /// <summary>
        /// HTTPレスポンスのステータスコード。
        /// </summary>
        public HttpStatusCode StatusCode { get; }

        #endregion
    }
}

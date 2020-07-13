using System;
using System.Net;

namespace SampleApiServer.Exceptions
{
    /// <summary>
    /// 認証失敗用の例外クラス。
    /// </summary>
    /// <remarks>HTTPレスポンスの場合、<see href="https://developer.mozilla.org/ja/docs/Web/HTTP/Status/401">401 Unauthorized</see>を返す。</remarks>
    public class UnauthorizedException : ApiException
    {
        // TODO: 使用する箇所は認証周りだけの筈なので、専用クラス作らなくてもいいかも？
        //       （ただし、現状専用クラスが無いと業務エラーコードが返せない。）

        /// <summary>
        /// 渡されたエラーメッセージで汎用の認証失敗の例外を生成する。
        /// </summary>
        /// <param name="message">エラーメッセージ。</param>
        public UnauthorizedException(string message) : base(ErrorCode.UNALTHORIZED, HttpStatusCode.Unauthorized, message)
        {
        }

        /// <summary>
        /// 渡されたエラーコードで認証失敗の例外を生成する。
        /// </summary>
        /// <param name="errorCode">エラーコード。</param>
        public UnauthorizedException(ErrorCode errorCode) : base(errorCode, HttpStatusCode.Unauthorized)
        {
        }

        /// <summary>
        /// 渡されたエラーコードとメッセージで認証失敗の例外を生成する。
        /// </summary>
        /// <param name="errorCode">エラーコード。</param>
        /// <param name="message">エラーメッセージ。</param>
        public UnauthorizedException(ErrorCode errorCode, string message) : base(errorCode, HttpStatusCode.Unauthorized, message)
        {
        }

        /// <summary>
        /// 渡されたエラーコードとメッセージと例外で認証失敗の例外を生成する。
        /// </summary>
        /// <param name="errorCode">エラーコード。</param>
        /// <param name="message">エラーメッセージ。</param>
        /// <param name="innerException">元になった例外。</param>
        public UnauthorizedException(ErrorCode errorCode, string message, Exception innerException) : base(errorCode, HttpStatusCode.Unauthorized, message, innerException)
        {
        }
    }
}

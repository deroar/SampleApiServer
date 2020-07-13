using System.Net;

namespace SampleApiServer.Exceptions
{
    /// <summary>
    /// データ未存在系エラー用の例外クラス。
    /// </summary>
    /// <remarks>HTTPレスポンスの場合、<see href="https://developer.mozilla.org/ja/docs/Web/HTTP/Status/404">404 Not Found</see>を返す。</remarks>
    public class NotFoundException : ApiException
    {
        /// <summary>
        /// 渡されたエラーメッセージで汎用のデータ未存在の例外を生成する。
        /// </summary>
        /// <param name="message">エラーメッセージ。</param>
        public NotFoundException(string message) : base(ErrorCode.NOT_FOUND, HttpStatusCode.NotFound, message)
        {
        }

        /// <summary>
        /// 渡されたエラーコードでデータ未存在の例外を生成する。
        /// </summary>
        /// <param name="errorCode">エラーコード。</param>
        public NotFoundException(ErrorCode errorCode) : base(errorCode, HttpStatusCode.NotFound)
        {
        }

        /// <summary>
        /// 渡されたエラーコードとメッセージでデータ未存在の例外を生成する。
        /// </summary>
        /// <param name="errorCode">エラーコード。</param>
        /// <param name="message">エラーメッセージ。</param>
        public NotFoundException(ErrorCode errorCode, string message) : base(errorCode, HttpStatusCode.NotFound, message)
        {
        }
    }
}

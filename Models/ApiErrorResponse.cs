namespace SampleApiServer.Models
{
    /// <summary>
    /// エラー時のレスポンス
    /// </summary>
    public class ApiErrorResponse
    {
        /// <summary>
        /// エラーコード。
        /// </summary>
        public int Error { get; set; }
    }
}

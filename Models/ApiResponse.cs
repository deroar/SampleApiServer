namespace SampleApiServer.Models
{
    /// <summary>
    /// 正常終了時のレスポンス
    /// </summary>
    public class ApiResponse
    {
        /// <summary>
        /// サーバの現時刻
        /// </summary>
        public long ServerTime { get; set; }

        /// <summary>
        /// レスポンスのhash値
        /// </summary>
        public string Hash { get; set; }

        /// <summary>
        /// エラーコード
        /// </summary>
        /// <remarks>
        /// 成功レスポンスを表すこのクラスでは常に0
        /// クライアントの例外ハンドリング用に正常系でも必要
        /// </remarks>
        public int Error { get; } = 0;

        /// <summary>
        /// APIのアクションの結果
        /// </summary>
        public object Result { get; set; }

        /// <summary>
        /// ApiResponseのコンストラクタ
        /// </summary>
        /// <param name="serverTime">サーバの現時刻</param>
        /// <param name="hash">レスポンスのhash値</param>
        /// <param name="result">APIのアクションの結果</param>
        public ApiResponse(long serverTime, string hash, object result)
        {
            ServerTime = serverTime;
            Hash = hash;
            Result = result;
        }
    }
}

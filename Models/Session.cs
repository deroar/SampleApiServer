using System;

namespace SampleApiServer.Models
{
    /// <summary>
    /// セッション情報。
    /// </summary>
    public class Session
    {
        /// <summary>
        /// セッションID。
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 最終ログイン日時。
        /// </summary>
        public DateTimeOffset LastLogin { get; set; }
    }
}

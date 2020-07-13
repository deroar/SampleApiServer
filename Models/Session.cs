using System;

namespace SampleApiServer.Models
{
    /// <summary>
    /// セッション情報。
    /// </summary>
    /// <remarks>
    /// NGEでは普通のセッションを使わずに独自のセッション管理をしているので、
    /// そのセッション情報を扱うためのDTO。
    /// </remarks>
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

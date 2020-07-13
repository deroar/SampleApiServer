using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SampleApiServer.Models
{
    /// <summary>
    /// デバッグ情報を含むエラー時のレスポンス
    /// </summary>
    public class ApiErrorDebugResponse
    {
        /// <summary>
        /// エラーコード。ErrorCodeのint表現
        /// </summary>
        public int Error { get; set; }

        /// <summary>
        /// デバッグ情報
        /// </summary>
        public string DebugMessage { get; set; }
    }
}

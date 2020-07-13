using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SampleApiServer.Exceptions
{
    public enum ErrorCode : int
    {
        #region 汎用エラー

        /// <summary>
        /// 汎用の入力値不正。
        /// </summary>
        BAD_REQUEST = 10400,

        /// <summary>
        /// 認証失敗。
        /// </summary>
        UNALTHORIZED = 10401,

        /// <summary>
        /// 汎用のデータ未存在。
        /// </summary>
        NOT_FOUND = 10402,

        #endregion

        #region 未定義
        /// <summary>
        /// 未定義のエラー
        /// </summary>
        UNKNOWN = 99999,

        #endregion
    }
}

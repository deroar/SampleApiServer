using System;
using System.Text.RegularExpressions;

namespace SampleApiServer.Util.Swagger
{
    /// <summary>
    /// Swagger用のString拡張メソッド
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// {}で囲まれた部分以外をすべて小文字にする
        /// </summary>
        /// <param name="str">元の文字列</param>
        /// <returns>{}以外の部分が小文字になった文字列</returns>
        public static string ToLowerExceptBetweenBraces(this string str)
        {
            // この正規表現は{}に囲まれた部分以外にマッチする。
            // "some{example}string" => some, string
            Regex rx = new Regex("(?:^|})(.*?)(?:{|$)");
            return rx.Replace(str, match => match.Value.ToLower());
        }
    }
}

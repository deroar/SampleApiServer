using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using SampleApiServer.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SampleApiServer.Util
{
    /// <summary>
    /// HttpContextクラスの拡張メソッド
    /// </summary>
    public static class HttpContextExtensions
    {
        private const string PlayerIdClaimType = "PlayerId";
        private const string Delim = ":";

        /// <summary>
        /// プレイヤーID設定
        /// </summary>
        /// <param name="httpContext">HttpContext</param>
        /// <param name="playerId">認証するプレイヤーID</param>
        /// <param name="sessionId">セッションID</param>
        /// <returns></returns>
        public static async Task SetPlayerIdAndSessionIdAsync(this HttpContext httpContext, long playerId, string sessionId)
        {
            string payload = string.Join(Delim, playerId, sessionId);

            var claimsIdentity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
            claimsIdentity.AddClaim(new Claim(PlayerIdClaimType, payload));
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true,
            };

            await httpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal, authProperties);
        }

        /// <summary>
        /// プレイヤーID取得
        /// </summary>
        /// <param name="httpContext">HttpContext</param>
        /// <returns>プレイヤーID</returns>
        public static async Task<long> GetPlayerIdAsync(this HttpContext httpContext)
        {
            var claim = httpContext.User.FindFirst(PlayerIdClaimType);
            if (claim == null)
            {
                throw new NotFoundException("Session is not found");
            }
            return await Task.FromResult(Parse(claim.Value).playerId);
        }

        /// <summary>
        /// プレイヤーIDを取得する。
        /// </summary>
        /// <param name="httpContext">HTTPコンテキスト。</param>
        /// <returns>プレイヤーID。未認証の場合はnull。</returns>
        public static long? GetPlayerIdOrNull(this HttpContext httpContext)
        {
            var claim = httpContext.User.FindFirst(PlayerIdClaimType);
            return claim?.Value.Parse().playerId;
        }

        /// <summary>
        /// セッションID取得。セッションがない場合はnullを返す
        /// </summary>
        /// <param name="httpContext">HttpContext</param>
        /// <returns>セッションID</returns>
        public static string GetSessionIdOrNull(this HttpContext httpContext)
        {
            var claim = httpContext.User.FindFirst(PlayerIdClaimType);
            return claim?.Value.Parse().sessionId;
        }

        /// <summary>
        /// プレイヤーIDとセッションIDを取得する。
        /// </summary>
        /// <param name="httpContext">HTTPコンテキスト。</param>
        /// <returns>(long プレイヤーID, string セッションID)のタプル</returns>
        /// <exception cref="NGEUnauthorizedException">認証されていない場合。</exception>
        public static (long playerId, string sessionId) GetPlayerIdAndSessionId(this HttpContext httpContext)
        {
            var claim = httpContext.User.FindFirst(PlayerIdClaimType);
            if (claim == null)
            {
                throw new UnauthorizedException("Session is not found");
            }
            return claim.Value.Parse();
        }

        // payloadをplayerIdとsessionIdに分解する
        private static (long playerId, string sessionId) Parse(this string payload)
        {
            string[] splitted = payload.Split(Delim);
            if (splitted.Length != 2)
            {
                throw new UnauthorizedException("Session is not valid");
            }
            return (long.Parse(splitted[0]), splitted[1]);
        }
    }
}

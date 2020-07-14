using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SampleApiServer.Models;
using SampleApiServer.Services;
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace SampleApiServer.Controllers.players
{
    /// <summary>
    /// プレイヤーコントローラ
    /// </summary>
    public partial class PlayersController : ControllerBase
    {
        /// <summary>
        /// PostLoginのリクエスト
        /// </summary>
        public class PostLoginRequest
        {
            /// <summary>
            /// プレイヤーID
            /// </summary>
            [Required]
            public long Id { get; set; }
            /// <summary>
            /// デバイスID
            /// </summary>
            [Required]
            [MaxLength(32)]
            public string DeviceId { get; set; }
            /// <summary>
            /// プレイヤーUID
            /// </summary>
            [Required]
            public string PlayerUid { get; set; }
        }

        /// <summary>
        /// PostLoginのレスポンス
        /// </summary>
        public class PostLoginResponse
        {
            /// <summary>
            /// アセットデータのURL
            /// </summary>
            public string AssetsUrl { get; set; }
        }

        /// <summary>
        /// ログインし、日ごと処理を実行する
        /// </summary>
        /// <param name="request">リクエスト</param>
        /// <param name="loginService">DI</param>
        /// <param name="serviceLocation">DI</param>
        /// <returns>レスポンス</returns>
        [HttpPost("Login")]
        [AllowAnonymous]
        public async Task<PostLoginResponse> PostLogin(
            [FromBody] PostLoginRequest request,
            [FromServices] LoginService loginService,
            [FromServices] IOptions<ServiceLocation> serviceLocation
        )
        {
            var current = DateTimeOffset.Now;
            using var transaction = transactionManager.BeginTransaction();
            await loginService.LoginAsync(request.Id, request.DeviceId, request.PlayerUid, current);
            await transaction.Commit();

            return new PostLoginResponse
            {
                AssetsUrl = serviceLocation.Value.AssetUrl,
            };
        }
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SampleApiServer.Services;
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace SampleApiServer.Controllers.players
{
    public partial class PlayersController : ControllerBase
    {
        /// <summary>
        /// PostCreateのリクエスト
        /// </summary>
        public class PostCreateRequest
        {
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

            /// <summary>
            /// プレイヤー名
            /// </summary>
            public string Name { get; set; }
        }

        /// <summary>
        /// PostCreateのレスポンス
        /// </summary>
        public class PostCreateResponse
        {
            /// <summary>
            /// プレイヤーのID
            /// </summary>
            public long Id { get; set; }
        }

        /// <summary>
        /// プレイヤー登録する
        /// </summary>
        /// <param name="request">リクエスト</param>
        /// <param name="registrationService">DI</param>
        /// <returns>レスポンス</returns>
        [HttpPost]
        [AllowAnonymous]
        public async Task<PostCreateResponse> PostCreate(
            [FromBody] PostCreateRequest request,
            [FromServices] RegistrationService registrationService
        )
        {
            using var transaction = transactionManager.BeginTransaction();
            var now = DateTimeOffset.Now;
            var playerId = await registrationService.RegisterAsync(request.DeviceId, request.PlayerUid, request.Name, now);
            var res = new PostCreateResponse
            {
                // TODO: 難読化
                Id = playerId,
            };
            await transaction.Commit();
            return res;
        }
    }
}

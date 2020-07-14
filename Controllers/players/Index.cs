using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SampleApiServer.Infra.Transaction;

namespace SampleApiServer.Controllers.players
{
    /// <summary>
    /// プレイヤーコントローラ
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public partial class PlayersController : ControllerBase
    {
        private readonly ITransactionManager transactionManager;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public PlayersController(ITransactionManager transactionManager)
        {
            this.transactionManager = transactionManager;
        }
    }
}

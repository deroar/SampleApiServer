﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace SampleApiServer.Controllers.players
{
    [Route("api/[controller]")]
    [ApiController]
    public partial class PlayersController : ControllerBase
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public PlayersController()
        {
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Wordle_hw.Controllers
{
    [Authorize]
    public class WordleController : ApiController
    {
        [HttpGet]
        [Route("api/wordle/play")]
        public IHttpActionResult Play()
        {
            return Ok("You are authenticated and playing Wordle!");
        }
    }

}

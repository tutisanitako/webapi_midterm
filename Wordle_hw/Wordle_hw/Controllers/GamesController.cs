using System.Linq;
using System.Security.Claims;
using System.Web.Http;
using Wordle_hw.Filters;
using Wordle_hw.Models.DTOs;
using Wordle_hw.Services;

namespace Wordle_hw.Controllers
{
    [JwtAuthorize]
    [RoutePrefix("api/games")]
    public class GamesController : ApiController
    {
        private readonly IGameService _gameService;

        public GamesController()
        {
            _gameService = new GameService();
        }

        [HttpGet]
        [Route("")]
        public IHttpActionResult GetGames([FromUri] int? id = null)
        {
            var userId = GetCurrentUserId();
            if (userId == null)
                return Unauthorized();

            if (id.HasValue)
            {
                var game = _gameService.GetGame(id.Value, userId.Value);
                if (game == null)
                    return NotFound();
                return Ok(game);
            }
            else
            {
                var games = _gameService.GetUserGames(userId.Value);
                return Ok(games);
            }
        }

        [HttpPost]
        [Route("start")]
        public IHttpActionResult StartGame()
        {
            var userId = GetCurrentUserId();
            if (userId == null)
                return Unauthorized();

            var newGame = _gameService.StartNewGame(userId.Value);
            return Ok(new
            {
                message = "Game started",
                gameId = newGame.Id,
                startDate = newGame.StartDate
            });
        }

        [HttpPost]
        [Route("{gameId}/guess")]
        public IHttpActionResult Guess(int gameId, [FromBody] GuessRequest guessRequest)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = GetCurrentUserId();
            if (userId == null)
                return Unauthorized();

            var evaluationResponse = _gameService.ProcessGuess(gameId, userId.Value, guessRequest);
            if (evaluationResponse == null)
                return BadRequest("Invalid guess or game not found/finished.");

            return Ok(new
            {
                message = "Guess processed",
                result = evaluationResponse.Result,
                color = evaluationResponse.Color
            });
        }

        private int? GetCurrentUserId()
        {
            var identity = User.Identity as ClaimsIdentity;
            var userIdClaim = identity?.FindFirst(ClaimTypes.NameIdentifier);

            if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
                return userId;

            return null;
        }
    }
}
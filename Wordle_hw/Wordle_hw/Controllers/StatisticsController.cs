using System.Linq;
using System.Security.Claims;
using System.Web.Http;
using Wordle_hw.Filters;
using Wordle_hw.Models;
using Wordle_hw.Models.DTOs;

namespace Wordle_hw.Controllers
{
    [JwtAuthorize]
    [RoutePrefix("api/stats")]
    public class StatisticsController : ApiController
    {
        private readonly AppDbContext _db;

        public StatisticsController()
        {
            _db = new AppDbContext();
        }

        [HttpGet]
        [Route("")]
        public IHttpActionResult GetStatistics()
        {
            var userId = GetCurrentUserId();
            if (userId == null)
                return Unauthorized();

            var stats = _db.Statistics.FirstOrDefault(s => s.UserId == userId.Value);
            if (stats == null)
            {
                return Ok(new StatisticsResponse
                {
                    GamesPlayed = 0,
                    Wins = 0,
                    CurrentStreak = 0,
                    MaxStreak = 0,
                    TotalPoints = 0
                });
            }

            return Ok(new StatisticsResponse
            {
                GamesPlayed = stats.GamesPlayed,
                Wins = stats.Wins,
                CurrentStreak = stats.CurrentStreak,
                MaxStreak = stats.MaxStreak,
                TotalPoints = stats.TotalPoints
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

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _db?.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
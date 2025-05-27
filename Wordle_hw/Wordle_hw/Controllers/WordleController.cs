using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Wordle_hw.Models;
using Wordle_hw.Services;
using System.Linq; // Make sure this is present for FirstOrDefault()
using System.Web.Security; // For FormsAuthentication (though not directly used in GetCurrentUserId logic here)
using System.Web.Http.Results;
using System.Threading.Tasks;
using Wordle_hw.Data; // <--- MAKE SURE THIS IS ADDED SO DbContext IS FOUND

// You might need to add:
// using Microsoft.AspNet.Identity; // If you use ASP.NET Identity for authentication

namespace Wordle_hw.Controllers
{
    // You'll need proper authentication setup to get a UserId.
    // For now, let's keep it without [Authorize] or simulate a user ID.
    // If you plan to implement user authentication, uncomment [Authorize] later.
    // [Authorize]
    public class WordleController : ApiController
    {
        private readonly GameManager _gameManager;

        public WordleController()
        {
            _gameManager = new GameManager();
        }

        // Helper to get User ID (now using authentication context and DB)
        private int? GetCurrentUserId()
        {
            if (User.Identity.IsAuthenticated)
            {
                string userEmail = User.Identity.Name; // This will be the email used to set the auth cookie
                using (var dbContext = new WordleDbContext()) // <--- Ensure WordleDbContext is accessible (using Wordle_hw.Data;)
                {
                    var user = dbContext.Users.FirstOrDefault(u => u.Email == userEmail);
                    return user?.Id; // Return Id if user found, null otherwise
                }
            }
            return null; // Not authenticated
        }


        /// <summary>
        /// Starts a new Wordle game.
        /// </summary>
        /// <returns>A response containing the GameId and initial attempts left.</returns>
        [HttpPost]
        [Route("api/games/start")]
        public IHttpActionResult StartGame()
        {
            try
            {
                int? userId = GetCurrentUserId(); // Get current user ID (can be null for anonymous)
                var response = _gameManager.StartNewGame(userId);
                return Ok(response);
            }
            catch (InvalidOperationException ex)
            {
                return InternalServerError(ex);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        /// <summary>
        /// Submits a guess for an active Wordle game.
        /// </summary>
        /// <param name="request">The guess request containing GameId and the guessed word.</param>
        /// <returns>A response indicating the status of each letter, attempts left, and game status.</returns>
        [HttpPost]
        [Route("api/games/guess")]
        public IHttpActionResult GuessWord([FromBody] GuessRequest request)
        {
            if (request == null || !ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var result = _gameManager.ProcessGuess(request.GameId, request.Guess);

                switch (result.Status)
                {
                    case "Game Not Found":
                        return NotFound();
                    case "Game Over":
                    case "Success": // Both won and lost games will have "Success" status internally if processed
                        return Ok(result);
                    case "Invalid Length":
                    case "Invalid Guess Format":
                        return BadRequest(result.Status);
                    default:
                        return InternalServerError(new Exception($"Unknown status: {result.Status}"));
                }
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        // --- New Endpoint for User Statistics ---
        [HttpGet]
        [Route("api/users/{userId}/statistics")]
        public IHttpActionResult GetUserStatistics(int userId)
        {
            // You'll want authorization here to ensure only the user or admin can see their stats
            // if (GetCurrentUserId() != userId && !User.IsInRole("Admin"))
            // {
            //     return Unauthorized();
            // }

            try
            {
                var stats = _gameManager.GetUserStatistics(userId);
                if (stats == null)
                {
                    return NotFound(); // No statistics found for this user
                }
                return Ok(stats);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Security;
using Wordle_hw.Data; // Assuming your DbContext is here
using Wordle_hw.Models; // For User, RegisterRequest, LoginRequest, Statistic
using System.Security.Cryptography; // For hashing
using System.Text; // For Encoding
using System.Web; // For HttpContext

namespace Wordle_hw.Controllers
{
    public class AccountController : ApiController
    {
        private string HashPassword(string password)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(password));
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }

        [HttpPost]
        [Route("api/register")]
        public IHttpActionResult Register([FromBody] RegisterRequest request)
        {
            if (request == null || !ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // --- PAY CLOSE ATTENTION HERE ---
            try // <--- This 'try' needs an opening brace right after it
            { // <--- MISSING THIS BRACE IS THE LIKELY CAUSE OF YOUR ERROR ON LINE 82
                using (var dbContext = new WordleDbContext())
                {
                    if (dbContext.Users.Any(u => u.Email == request.Email))
                    {
                        return BadRequest("Email already exists.");
                    }

                    var newUser = new User
                    {
                        Email = request.Email,
                        PasswordHash = HashPassword(request.Password)
                    };

                    dbContext.Users.Add(newUser);
                    dbContext.SaveChanges();

                    var newStatistic = new Statistic
                    {
                        UserId = newUser.Id,
                        GamesPlayed = 0,
                        Wins = 0,
                        CurrentStreak = 0,
                        MaxStreak = 0,
                        TotalPoints = 0
                    };
                    dbContext.Statistics.Add(newStatistic);
                    dbContext.SaveChanges();

                    return Ok("User registered successfully. Please log in.");
                }
            } // <--- And a closing brace here for the try block
            catch (Exception ex) // This is likely line 82 or close to it
            {
                return InternalServerError(ex);
            }
        }

        [HttpPost]
        [Route("api/login")]
        public IHttpActionResult Login([FromBody] LoginRequest request)
        {
            if (request == null || !ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // --- AND PAY CLOSE ATTENTION HERE ---
            try // <--- This 'try' needs an opening brace right after it
            { // <--- MISSING THIS BRACE IS THE LIKELY CAUSE OF YOUR ERROR ON LINE 119
                using (var dbContext = new WordleDbContext())
                {
                    var existingUser = dbContext.Users.FirstOrDefault(u => u.Email == request.Email);

                    if (existingUser == null)
                    {
                        return Unauthorized();
                    }

                    string hashedPassword = HashPassword(request.Password);
                    if (existingUser.PasswordHash != hashedPassword)
                    {
                        return Unauthorized();
                    }

                    FormsAuthentication.SetAuthCookie(existingUser.Email, false);

                    return Ok(new { Message = "Login successful", UserId = existingUser.Id, Email = existingUser.Email });
                }
            } // <--- And a closing brace here for the try block
            catch (Exception ex) // This is likely line 119 or close to it
            {
                return InternalServerError(ex);
            }
        }

        [HttpPost]
        [Route("api/logout")]
        public IHttpActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return Ok("Logged out successfully");
        }
    }
}
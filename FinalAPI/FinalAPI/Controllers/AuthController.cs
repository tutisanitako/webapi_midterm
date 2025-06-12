using System.Threading.Tasks;
using System.Web.Http;
using FinalAPI.Middleware;
using System.Configuration; // For ConfigurationManager
using System; // For ArgumentNullException
using System.Security.Claims; // Make sure this is present for Claims, ClaimTypes

namespace FinalAPI.Controllers
{
    [RoutePrefix("api/auth")]
    public class AuthController : ApiController
    {
        private readonly JwtTokenService _tokenService;

        public AuthController()
        {
            var secretKey = ConfigurationManager.AppSettings["JwtSecretKey"];
            var issuer = ConfigurationManager.AppSettings["JwtIssuer"];
            var audience = ConfigurationManager.AppSettings["JwtAudience"];

            _tokenService = new JwtTokenService(secretKey, issuer, audience);
        }

        [HttpPost]
        [Route("register")]
        [AllowAnonymous]
        public async Task<IHttpActionResult> Register(RegisterRequest registerRequest)
        {
            if (registerRequest == null || string.IsNullOrEmpty(registerRequest.Username) || string.IsNullOrEmpty(registerRequest.Password))
            {
                return BadRequest("Username and password are required.");
            }

            string role = "doctor"; 
            string userId = "3";

            int? doctorIdClaim = null;
            if (role == "doctor")
            {
                doctorIdClaim = 3;
            }

            if (registerRequest.Username == "admin" || registerRequest.Username == "doctor")
            {
                return BadRequest("Username already exists.");
            }

            var token = await _tokenService.GenerateTokenAsync(userId, role, doctorIdClaim);

            return Ok(new LoginResponse
            {
                Token = token,
                UserId = userId,
                Role = role,
                ExpiresIn = 3600
            });
        }

        [HttpPost]
        [Route("login")]
        [AllowAnonymous]
        public async Task<IHttpActionResult> Login(LoginRequest loginRequest)
        {
            if (loginRequest == null || string.IsNullOrEmpty(loginRequest.Username) || string.IsNullOrEmpty(loginRequest.Password))
            {
                return BadRequest("Username and password are required.");
            }

            string role = null;
            string userId = null;
            int? doctorIdClaim = null;

            if (loginRequest.Username == "admin" && loginRequest.Password == "admin123")
            {
                role = "admin";
                userId = "1";
            }
            else if (loginRequest.Username == "doctor" && loginRequest.Password == "doctor123")
            {
                role = "doctor";
                userId = "2";
                doctorIdClaim = 2;
            }
            else
            {
                return Unauthorized();
            }

            var token = await _tokenService.GenerateTokenAsync(userId, role, doctorIdClaim);

            return Ok(new LoginResponse
            {
                Token = token,
                UserId = userId,
                Role = role,
                ExpiresIn = 3600
            });
        }
    }

    public class RegisterRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public class LoginRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public class LoginResponse
    {
        public string Token { get; set; }
        public string UserId { get; set; }
        public string Role { get; set; }
        public int ExpiresIn { get; set; }
    }
}
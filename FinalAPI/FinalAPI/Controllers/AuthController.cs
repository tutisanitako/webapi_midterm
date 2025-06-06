using System.Threading.Tasks;
using System.Web.Http;
using System.Configuration;
using FinalAPI.Middleware;

namespace FinalAPI.Controllers
{
    /// <summary>
    /// Controller for authentication operations.
    /// Handles login, registration, and token generation.
    /// </summary>
    [RoutePrefix("api/auth")]
    public class AuthController : ApiController
    {
        private readonly JwtTokenService _tokenService;

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthController"/> class.
        /// </summary>
        public AuthController()
        {
            var secretKey = ConfigurationManager.AppSettings["JwtSecretKey"];
            var issuer = ConfigurationManager.AppSettings["JwtIssuer"];
            var audience = ConfigurationManager.AppSettings["JwtAudience"];

            _tokenService = new JwtTokenService(secretKey, issuer, audience);
        }

        /// <summary>
        /// Registers a new user and returns a JWT token.
        /// For demo purposes, using hardcoded validation.
        /// In production, this would save to a user database.
        /// </summary>
        /// <param name="registerRequest">Registration credentials</param>
        /// <returns>JWT token and user info</returns>
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

            if (registerRequest.Username == "admin" || registerRequest.Username == "doctor")
            {
                return BadRequest("Username already exists.");
            }

            var token = await _tokenService.GenerateTokenAsync(userId, role);

            return Ok(new LoginResponse
            {
                Token = token,
                UserId = userId,
                Role = role,
                ExpiresIn = 3600
            });
        }

        /// <summary>
        /// Authenticates a user and returns a JWT token.
        /// For demo purposes, using hardcoded credentials.
        /// In production, this would validate against a user database.
        /// </summary>
        /// <param name="loginRequest">Login credentials</param>
        /// <returns>JWT token and user info</returns>
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

            if (loginRequest.Username == "admin" && loginRequest.Password == "admin123")
            {
                role = "admin";
                userId = "1";
            }
            else if (loginRequest.Username == "doctor" && loginRequest.Password == "doctor123")
            {
                role = "doctor";
                userId = "2";
            }
            else
            {
                return Unauthorized();
            }

            var token = await _tokenService.GenerateTokenAsync(userId, role);

            return Ok(new LoginResponse
            {
                Token = token,
                UserId = userId,
                Role = role,
                ExpiresIn = 3600
            });
        }
    }

    /// <summary>
    /// Request model for user registration.
    /// </summary>
    public class RegisterRequest
    {
        /// <summary>
        /// Gets or sets the username for registration.
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// Gets or sets the password for registration.
        /// </summary>
        public string Password { get; set; }
    }

    /// <summary>
    /// Request model for user login.
    /// </summary>
    public class LoginRequest
    {
        /// <summary>
        /// Gets or sets the username for login.
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// Gets or sets the password for login.
        /// </summary>
        public string Password { get; set; }
    }

    /// <summary>
    /// Response model for successful login or registration.
    /// </summary>
    public class LoginResponse
    {
        /// <summary>
        /// Gets or sets the JWT token.
        /// </summary>
        public string Token { get; set; }

        /// <summary>
        /// Gets or sets the user ID.
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// Gets or sets the user role.
        /// </summary>
        public string Role { get; set; }

        /// <summary>
        /// Gets or sets the token expiration time in seconds.
        /// </summary>
        public int ExpiresIn { get; set; }
    }
}

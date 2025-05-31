using System.Web.Http;
using Wordle_hw.Models.DTOs;
using Wordle_hw.Services;

namespace Wordle_hw.Controllers
{
    [RoutePrefix("api/auth")]
    public class AuthController : ApiController
    {
        private readonly IUserService _userService;

        public AuthController()
        {
            _userService = new UserService();
        }

        [HttpPost]
        [Route("register")]
        public IHttpActionResult Register([FromBody] RegisterModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var success = _userService.Register(model);
            if (!success)
                return BadRequest("Username or email already exists.");

            return Ok(new { message = "Registration successful." });
        }

        [HttpPost]
        [Route("login")]
        public IHttpActionResult Login([FromBody] LoginModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = _userService.Authenticate(model);
            if (user == null)
                return BadRequest("Invalid username or password.");

            var token = _userService.GenerateJwtToken(user);

            return Ok(new
            {
                message = "Login successful.",
                token = "Bearer " + token,
                user = new
                {
                    id = user.Id,
                    username = user.Username,
                    email = user.Email
                }
            });
        }
    }
}
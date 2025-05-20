using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Security;
using Wordle_hw.Models;

public class AccountController : ApiController
{
    private static List<User> users = new List<User>();

    [HttpPost]
    [Route("api/register")]
    public IHttpActionResult Register([FromBody] User user)
    {
        if (users.Any(u => u.Username == user.Username))
            return BadRequest("Username already exists");

        users.Add(user);
        return Ok("User registered successfully");
    }

    [HttpPost]
    [Route("api/login")]
    public IHttpActionResult Login(User user)
    {
        var existingUser = users.FirstOrDefault(u => u.Username == user.Username && u.Password == user.Password);
        if (existingUser == null)
            return Unauthorized();

        FormsAuthentication.SetAuthCookie(user.Username, false);
        return Ok("Login successful");
    }

    [HttpPost]
    [Route("api/logout")]
    public IHttpActionResult Logout()
    {
        FormsAuthentication.SignOut();
        return Ok("Logged out");
    }
}

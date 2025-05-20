using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WordleAuthApi.Models;

namespace WordleAuthApi.Controllers
{
    public class AuthController : ApiController
    {
        private UserDbContext db = new UserDbContext();

        [HttpPost]
        [Route("api/register")]
        public IHttpActionResult Register(User user)
        {
            if (db.Users.Any(u => u.Username == user.Username))
                return BadRequest("Username already exists");

            db.Users.Add(user);
            db.SaveChanges();

            return Ok("Registered successfully");
        }

        [HttpPost]
        [Route("api/login")]
        public IHttpActionResult Login(User user)
        {
            var existingUser = db.Users.FirstOrDefault(u => u.Username == user.Username && u.Password == user.Password);
            if (existingUser == null)
                return Unauthorized();

            return Ok("Login successful");
        }
    }
}

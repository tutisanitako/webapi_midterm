using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Wordle_hw.Models;
using Wordle_hw.Models.DTOs;
using Wordle_hw.Models.Entities;

namespace Wordle_hw.Services
{
    public class UserService : IUserService
    {
        private readonly AppDbContext _db;
        private readonly string _jwtKey;
        private readonly string _jwtIssuer;
        private readonly string _jwtAudience;
        private readonly int _jwtExpiryInMinutes;

        public UserService()
        {
            _db = new AppDbContext();
            // Retrieve JWT configuration values from App.config or Web.config
            _jwtKey = System.Configuration.ConfigurationManager.AppSettings["JwtKey"];
            _jwtIssuer = System.Configuration.ConfigurationManager.AppSettings["JwtIssuer"];
            _jwtAudience = System.Configuration.ConfigurationManager.AppSettings["JwtAudience"];
            // Parse expiry time, defaulting to 60 minutes if not specified
            _jwtExpiryInMinutes = int.Parse(System.Configuration.ConfigurationManager.AppSettings["JwtExpiryInMinutes"] ?? "60");
        }

        public bool Register(RegisterModel model)
        {
            // Check if a user with the same username or email already exists
            if (_db.Users.Any(u => u.Username == model.Username || u.Email == model.Email))
            {
                return false; // Registration failed: user already exists
            }

            // Create a new User entity
            var user = new User
            {
                Username = model.Username,
                // Hash the password using BCrypt for security
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password),
                Email = model.Email
            };

            // Add the new user to the database context
            _db.Users.Add(user);
            _db.SaveChanges(); // Save changes to get the auto-generated User.Id

            // Create an initial Statistics record for the new user
            var stats = new Statistic
            {
                // Link the Statistic to the newly created User using UserId as the foreign key
                UserId = user.Id,
                GamesPlayed = 0,
                Wins = 0,
                CurrentStreak = 0,
                MaxStreak = 0,
                TotalPoints = 0
            };

            // Add the new statistics record to the database context
            _db.Statistics.Add(stats);
            _db.SaveChanges(); // Save changes to persist the statistics

            return true; // Registration successful
        }

        public User Authenticate(LoginModel model)
        {
            // Find user by username
            var user = _db.Users.FirstOrDefault(u => u.Username == model.Username);

            // Check if user exists and password is correct
            if (user == null || !BCrypt.Net.BCrypt.Verify(model.Password, user.PasswordHash))
            {
                return null; // Authentication failed
            }
            return user; // Authentication successful
        }

        public User GetUserById(int userId)
        {
            return _db.Users.FirstOrDefault(u => u.Id == userId);
        }

        public string GenerateJwtToken(User user)
        {
            // Define claims for the JWT
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Username), // Subject
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()), // JWT ID
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()), // User ID
                new Claim(ClaimTypes.Name, user.Username) // User Name
            };

            // Create security key using the configured JWT secret key
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtKey));
            // Create signing credentials with HMAC SHA256 algorithm
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // Create the JWT
            var token = new JwtSecurityToken(
                issuer: _jwtIssuer,
                audience: _jwtAudience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_jwtExpiryInMinutes), // Set token expiry time
                signingCredentials: creds);

            // Serialize and return the token as a string
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        // Dispose method for cleaning up the DbContext
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _db?.Dispose(); // Dispose the DbContext if it's not null
            }
        }

        // Public Dispose method to implement IDisposable
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this); // Prevent the finalizer from running
        }
    }
}
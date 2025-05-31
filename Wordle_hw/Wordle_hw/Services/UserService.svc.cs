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
            _jwtKey = System.Configuration.ConfigurationManager.AppSettings["JwtKey"];
            _jwtIssuer = System.Configuration.ConfigurationManager.AppSettings["JwtIssuer"];
            _jwtAudience = System.Configuration.ConfigurationManager.AppSettings["JwtAudience"];
            _jwtExpiryInMinutes = int.Parse(System.Configuration.ConfigurationManager.AppSettings["JwtExpiryInMinutes"] ?? "60");
        }

        public bool Register(RegisterModel model)
        {
            if (_db.Users.Any(u => u.Username == model.Username || u.Email == model.Email))
            {
                return false;
            }

            var user = new User
            {
                Username = model.Username,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password),
                Email = model.Email
            };

            _db.Users.Add(user);
            _db.SaveChanges();

            var stats = new Statistic
            {

                UserId = user.Id,
                GamesPlayed = 0,
                Wins = 0,
                CurrentStreak = 0,
                MaxStreak = 0,
                TotalPoints = 0
            };

            _db.Statistics.Add(stats);
            _db.SaveChanges();

            return true;
        }

        public User Authenticate(LoginModel model)
        {
            var user = _db.Users.FirstOrDefault(u => u.Username == model.Username);

            if (user == null || !BCrypt.Net.BCrypt.Verify(model.Password, user.PasswordHash))
            {
                return null;
            }
            return user;
        }

        public User GetUserById(int userId)
        {
            return _db.Users.FirstOrDefault(u => u.Id == userId);
        }

        public string GenerateJwtToken(User user)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Username), 
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()), 
                new Claim(ClaimTypes.Name, user.Username)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _jwtIssuer,
                audience: _jwtAudience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_jwtExpiryInMinutes),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _db?.Dispose();
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this); 
        }
    }
}
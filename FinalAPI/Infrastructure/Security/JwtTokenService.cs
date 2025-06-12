using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace FinalAPI.Middleware
{
    /// <summary>
    /// Service for generating and validating JSON Web Tokens (JWTs).
    /// </summary>
    public class JwtTokenService
    {
        private readonly string _secretKey;
        private readonly string _issuer;
        private readonly string _audience;

        /// <summary>
        /// Initializes a new instance of the <see cref="JwtTokenService"/> class.
        /// </summary>
        /// <param name="secretKey">The secret key used for signing the JWT.</param>
        /// <param name="issuer">The issuer of the JWT.</param>
        /// <param name="audience">The audience for the JWT.</param>
        public JwtTokenService(string secretKey, string issuer, string audience)
        {
            _secretKey = secretKey;
            _issuer = issuer;
            _audience = audience;
        }

        /// <summary>
        /// Generates a JWT for the specified user ID, role, and optional DoctorId.
        /// </summary>
        /// <param name="userId">The unique identifier of the user (e.g., "1", "2", "3").</param>
        /// <param name="role">The role of the user (e.g., "admin", "doctor").</param>
        /// <param name="doctorId">Optional: The associated Doctor ID from the database, if the user is a doctor.</param>
        /// <returns>A task that represents the asynchronous operation, containing the generated JWT as a string.</returns>
        public async Task<string> GenerateTokenAsync(string userId, string role, int? doctorId = null)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, userId),
                new Claim(ClaimTypes.Role, role),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            if (doctorId.HasValue)
            {
                claims.Add(new Claim("DoctorId", doctorId.Value.ToString()));
            }

            var token = new JwtSecurityToken(
                issuer: _issuer,
                audience: _audience,
                claims: claims,
                expires: DateTime.Now.AddHours(1),
                signingCredentials: creds);

            return await Task.FromResult(new JwtSecurityTokenHandler().WriteToken(token));
        }

        /// <summary>
        /// Validates a JWT and returns the associated claims principal.
        /// </summary>
        /// <param name="token">The JWT to validate.</param>
        /// <returns>A task that represents the asynchronous operation, containing the validated <see cref="ClaimsPrincipal"/> if successful; otherwise, null.</returns>
        public async Task<ClaimsPrincipal> ValidateTokenAsync(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_secretKey);

            try
            {
                var principal = await Task.FromResult(tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = _issuer,
                    ValidAudience = _audience,
                    IssuerSigningKey = new SymmetricSecurityKey(key)
                }, out _));
                return principal;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Token validation failed: {ex.ToString()}");
                return null;
            }
        }
    }
}
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Web.Http;
using System.Web.Http.Controllers;
using Microsoft.IdentityModel.Tokens;

namespace Wordle_hw.Filters
{
    public class JwtAuthorizeAttribute : AuthorizeAttribute
    {
        private readonly string _issuer = System.Configuration.ConfigurationManager.AppSettings["JwtIssuer"];
        private readonly string _audience = System.Configuration.ConfigurationManager.AppSettings["JwtAudience"];
        private readonly string _key = System.Configuration.ConfigurationManager.AppSettings["JwtKey"];

        protected override bool IsAuthorized(HttpActionContext actionContext)
        {
            try
            {
                if (!actionContext.Request.Headers.Authorization?.Scheme.Equals("Bearer", StringComparison.OrdinalIgnoreCase) ?? true)
                {
                    System.Diagnostics.Trace.WriteLine("Authorization header missing or not Bearer");
                    return false;
                }

                var token = actionContext.Request.Headers.Authorization.Parameter;
                if (string.IsNullOrEmpty(token))
                {
                    System.Diagnostics.Trace.WriteLine("Token is empty");
                    return false;
                }

                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.UTF8.GetBytes(_key);

                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = _issuer,
                    ValidAudience = _audience,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ClockSkew = TimeSpan.FromMinutes(5)
                }, out var validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;
                actionContext.RequestContext.Principal = new ClaimsPrincipal(new ClaimsIdentity(jwtToken.Claims, "jwt"));
                System.Diagnostics.Trace.WriteLine("Token validated successfully");
                return true;
            }
            catch (SecurityTokenExpiredException ex)
            {
                System.Diagnostics.Trace.WriteLine($"Token expired: {ex.Message}");
                return false;
            }
            catch (SecurityTokenInvalidSignatureException ex)
            {
                System.Diagnostics.Trace.WriteLine($"Invalid signature: {ex.Message}");
                return false;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine($"Token validation failed: {ex.Message}");
                return false;
            }
        }
    }
}
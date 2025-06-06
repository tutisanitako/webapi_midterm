using FinalAPI.Middleware;
using System;
using System.Configuration;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Filters;
using System.Web.Http.Results;

namespace FinalAPI.Middleware
{
    /// <summary>
    /// Middleware to handle JWT authentication for Web API.
    /// Validates the JWT token and sets the user principal.
    /// </summary>
    public class JwtAuthMiddleware : IAuthenticationFilter
    {
        private readonly JwtTokenService _tokenService;

        /// <summary>
        /// Initializes a new instance of the <see cref="JwtAuthMiddleware"/> class.
        /// </summary>
        public JwtAuthMiddleware()
        {
            var secretKey = ConfigurationManager.AppSettings["JwtSecretKey"];
            var issuer = ConfigurationManager.AppSettings["JwtIssuer"];
            var audience = ConfigurationManager.AppSettings["JwtAudience"];
            _tokenService = new JwtTokenService(secretKey, issuer, audience);
        }

        /// <summary>
        /// Gets a value indicating whether multiple authentication filters are allowed.
        /// </summary>
        public bool AllowMultiple => false;

        /// <summary>
        /// Authenticates the request by validating the JWT token.
        /// </summary>
        /// <param name="context">The authentication context.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task AuthenticateAsync(HttpAuthenticationContext context, CancellationToken cancellationToken)
        {
            var request = context.Request;
            if (request.RequestUri.AbsolutePath.EndsWith("/api/auth/login", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            var authorizationHeader = request.Headers.Authorization;
            if (authorizationHeader == null || authorizationHeader.Scheme != "Bearer" || string.IsNullOrEmpty(authorizationHeader.Parameter))
            {
                context.ErrorResult = new UnauthorizedResult(new System.Net.Http.Headers.AuthenticationHeaderValue[0], request);
                return;
            }

            var token = authorizationHeader.Parameter;
            try
            {
                var principal = await _tokenService.ValidateTokenAsync(token);
                if (principal == null)
                {
                    context.ErrorResult = new UnauthorizedResult(new System.Net.Http.Headers.AuthenticationHeaderValue[0], request);
                    return;
                }
                context.Principal = principal;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Token validation failed: {ex.Message}");
                context.ErrorResult = new UnauthorizedResult(new System.Net.Http.Headers.AuthenticationHeaderValue[0], request);
            }
        }

        /// <summary>
        /// Adds a challenge to the response if authentication fails.
        /// </summary>
        /// <param name="context">The authentication challenge context.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public Task ChallengeAsync(HttpAuthenticationChallengeContext context, CancellationToken cancellationToken)
        {
            var result = context.Result;
            if (result is UnauthorizedResult)
            {
                context.Result = new AddChallengeOnUnauthorizedResult(
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", "realm=\"api\""),
                    result);
            }
            return Task.CompletedTask;
        }
    }

    /// <summary>
    /// Helper class to add WWW-Authenticate header on unauthorized responses.
    /// </summary>
    public class AddChallengeOnUnauthorizedResult : IHttpActionResult
    {
        private readonly System.Net.Http.Headers.AuthenticationHeaderValue _challenge;
        private readonly IHttpActionResult _innerResult;

        /// <summary>
        /// Initializes a new instance of the <see cref="AddChallengeOnUnauthorizedResult"/> class.
        /// </summary>
        /// <param name="challenge">The authentication challenge to add.</param>
        /// <param name="innerResult">The inner HTTP action result.</param>
        public AddChallengeOnUnauthorizedResult(System.Net.Http.Headers.AuthenticationHeaderValue challenge, IHttpActionResult innerResult)
        {
            _challenge = challenge;
            _innerResult = innerResult;
        }

        /// <summary>
        /// Executes the result and adds the WWW-Authenticate header if unauthorized.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task that represents the asynchronous operation, containing the HTTP response message.</returns>
        public async Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            var response = await _innerResult.ExecuteAsync(cancellationToken);
            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                response.Headers.WwwAuthenticate.Add(_challenge);
            }
            return response;
        }
    }
}
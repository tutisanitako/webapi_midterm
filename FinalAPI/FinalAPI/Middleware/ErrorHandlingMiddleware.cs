using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.ExceptionHandling;
using System.Web.Http.Results;

namespace FinalAPI.Middleware
{
    /// <summary>
    /// Global exception handler middleware for Web API.
    /// Catches unhandled exceptions and returns a standardized error response.
    /// </summary>
    public class ErrorHandlingMiddleware : ExceptionHandler
    {
        /// <summary>
        /// Handles unhandled exceptions and returns a standardized error response.
        /// </summary>
        /// <param name="context">The exception handler context.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public override Task HandleAsync(ExceptionHandlerContext context, CancellationToken cancellationToken)
        {
            Console.WriteLine($"Unhandled exception: {context.Exception}");

            var errorResponse = new
            {
                Message = "An unexpected error occurred.",
                Details = context.Exception.Message,
                StatusCode = (int)HttpStatusCode.InternalServerError
            };

            context.Result = new JsonResult<object>(errorResponse, context.Request);

            return Task.CompletedTask;
        }
    }

    /// <summary>
    /// Custom JsonResult to return a JSON response with the specified content.
    /// </summary>
    /// <typeparam name="T">The type of the content.</typeparam>
    public class JsonResult<T> : IHttpActionResult
    {
        private readonly T _content;
        private readonly HttpRequestMessage _request;

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonResult{T}"/> class.
        /// </summary>
        /// <param name="content">The content to return in the response.</param>
        /// <param name="request">The HTTP request message.</param>
        public JsonResult(T content, HttpRequestMessage request)
        {
            _content = content;
            _request = request;
        }

        /// <summary>
        /// Executes the result and returns an HTTP response message.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task that represents the asynchronous operation, containing the HTTP response message.</returns>
        public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            var response = new HttpResponseMessage(HttpStatusCode.InternalServerError)
            {
                Content = new ObjectContent<T>(_content, new System.Net.Http.Formatting.JsonMediaTypeFormatter()),
                RequestMessage = _request
            };
            return Task.FromResult(response);
        }
    }
}
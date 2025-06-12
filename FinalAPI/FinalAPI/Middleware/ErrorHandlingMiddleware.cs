using Application.Services;
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
    public class ErrorHandlingMiddleware : IExceptionHandler
    {
        public async Task HandleAsync(ExceptionHandlerContext context, CancellationToken cancellationToken)
        {
            var exception = context.Exception;

            Console.WriteLine($"Error: {exception.Message}\nStackTrace: {exception.StackTrace}");

            if (exception is AppServiceException)
            {
                context.Result = new ErrorMessageResult(context.Request, HttpStatusCode.BadRequest, exception.Message);
            }
            else
            {
                context.Result = new ErrorMessageResult(context.Request, HttpStatusCode.InternalServerError,
                    $"An unexpected error occurred: {exception.Message} (See logs for details)");
            }

            await Task.CompletedTask;
        }

        private class ErrorMessageResult : IHttpActionResult
        {
            private readonly HttpRequestMessage _request;
            private readonly HttpStatusCode _statusCode;
            private readonly string _message;

            public ErrorMessageResult(HttpRequestMessage request, HttpStatusCode statusCode, string message)
            {
                _request = request;
                _statusCode = statusCode;
                _message = message;
            }

            public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
            {
                var response = _request.CreateResponse(_statusCode, new { error = _message });
                return Task.FromResult(response);
            }
        }
    }
}
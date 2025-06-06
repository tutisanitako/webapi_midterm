using System;

namespace Application.Services
{
    /// <summary>
    /// Base class for custom exceptions in the application layer.
    /// This exception is designed to be caught and handled gracefully in the presentation layer.
    /// </summary>
    public class AppServiceException : Exception
    {
        public AppServiceException(string message) : base(message) { }
    }
}
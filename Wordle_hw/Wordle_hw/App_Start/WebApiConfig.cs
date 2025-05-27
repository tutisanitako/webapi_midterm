using System.Web.Http;
// Add these two usings if they are not already there
using System.Net.Http.Formatting;
using System.Net.Http.Headers;

namespace Wordle_hw
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services

            // This line is crucial for enabling attribute routing ([Route(...)])
            config.MapHttpAttributeRoutes();

            // Web API routes
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            // --- ENSURE THESE LINES ARE PRESENT AND UNCOMMENTED ---
            // Add JSON formatter if it's somehow missing or misconfigured
            config.Formatters.Clear(); // Clear existing formatters
            config.Formatters.Add(new JsonMediaTypeFormatter()); // Add JSON formatter

            // You might also want to ensure default headers for JSON
            config.Formatters.JsonFormatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue("text/html")); // This helps if some clients send text/html but expect JSON
            // Remove the line above if it causes issues, usually not needed but sometimes helps in older setups
            // The main goal is to ensure application/json is handled.
        }
    }
}
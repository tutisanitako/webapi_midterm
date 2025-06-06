using FinalAPI.Middleware;
using System.Web.Http.ExceptionHandling;
using System.Web.Http;

namespace FinalAPI.App_Start
{
    /// <summary>
    /// Configures Web API settings and routes.
    /// </summary>
    public static class WebApiConfig
    {
        /// <summary>
        /// Registers Web API configuration and routes.
        /// </summary>
        /// <param name="config">The HTTP configuration to set up.</param>
        public static void Register(HttpConfiguration config)
        {
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            config.Services.Replace(typeof(IExceptionHandler), new ErrorHandlingMiddleware());
        }
    }
}
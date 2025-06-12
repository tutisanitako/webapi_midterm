using System.Web.Http;
using FinalAPI.Middleware;
using System.Web.Http.ExceptionHandling;

namespace FinalAPI
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            config.MapHttpAttributeRoutes();

            config.Services.Replace(typeof(IExceptionHandler), new ErrorHandlingMiddleware());

            config.Filters.Add(new JwtAuthMiddleware());

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}
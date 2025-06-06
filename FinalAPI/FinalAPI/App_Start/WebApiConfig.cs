using System;
using System.Web.Http;
using FinalAPI.Middleware;

namespace FinalAPI
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Ensure DI container is configured
            DependencyConfig.ConfigureServices();

            // Enable attribute routing
            config.MapHttpAttributeRoutes();

            // Resolve JwtTokenService from DI container
            var tokenService = config.DependencyResolver.GetService(typeof(JwtTokenService)) as JwtTokenService
                ?? throw new InvalidOperationException("JwtTokenService could not be resolved from the DI container");
            config.Filters.Add(new JwtAuthMiddleware(tokenService));

            // Default route
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}
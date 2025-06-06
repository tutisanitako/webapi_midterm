using System.Web.Mvc;
using System.Web.Routing;

namespace FinalAPI.App_Start
{
    /// <summary>
    /// Configures routing for the ASP.NET MVC application.
    /// </summary>
    public class RouteConfig
    {
        /// <summary>
        /// Registers routes for the application.
        /// </summary>
        /// <param name="routes">The collection of routes to configure.</param>
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
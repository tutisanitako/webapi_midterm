using System.Web.Mvc;

namespace FinalAPI.App_Start
{
    /// <summary>
    /// Configures global filters for the ASP.NET MVC application.
    /// </summary>
    public class FilterConfig
    {
        /// <summary>
        /// Registers global filters for the application.
        /// </summary>
        /// <param name="filters">The collection of global filters to configure.</param>
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
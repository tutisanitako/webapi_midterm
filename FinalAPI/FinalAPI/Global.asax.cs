using System.Web.Http;
using FinalAPI.App_Start;

namespace FinalAPI
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);
            // DependencyConfig.ConfigureServices(); // Remove if called in WebApiConfig.cs
        }
    }
}
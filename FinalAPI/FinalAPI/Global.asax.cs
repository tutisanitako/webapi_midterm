using System.Web.Http;

namespace FinalAPI
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);
            //DependencyConfig.ConfigureServices();
            // SwaggerConfig.Register is already called via WebActivatorEx
        }
    }
}
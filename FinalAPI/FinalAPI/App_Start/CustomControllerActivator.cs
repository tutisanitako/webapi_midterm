using System;
using System.Net.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Dispatcher;
using Microsoft.Extensions.DependencyInjection;

namespace FinalAPI.App_Start
{
    /// <summary>
    /// Custom IHttpControllerActivator to integrate with Microsoft.Extensions.DependencyInjection.
    /// This allows Web API to resolve controllers and their dependencies from the configured DI container.
    /// </summary>
    public class CustomControllerActivator : IHttpControllerActivator
    {
        private readonly IServiceProvider _serviceProvider;

        public CustomControllerActivator(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }

        public IHttpController Create(HttpRequestMessage request, HttpControllerDescriptor controllerDescriptor, Type controllerType)
        {

            var scope = request.GetDependencyScope();
            if (scope == null)
            {
                return _serviceProvider.GetService(controllerType) as IHttpController;
            }

            return scope.GetService(controllerType) as IHttpController;
        }
    }
}
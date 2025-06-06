using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Data;
using Infrastructure.Repositories;
using Application.Services;
using Microsoft.Extensions.DependencyInjection;
using System.Data.Entity;
using System.Web.Http;
using System.Web.Mvc;
using System;
using System.Configuration;
using FinalAPI.Middleware;
using System.Web.Http.Dependencies;

namespace FinalAPI
{
    public static class DependencyConfig
    {
        private static IServiceProvider _serviceProvider;

        public static void ConfigureServices()
        {
            var services = new ServiceCollection();

            // Register DbContext
            services.AddScoped<DbContext, HealthDbContext>();

            // Register Repositories
            services.AddScoped<IRepository<Patient>, PatientRepository>();
            services.AddScoped<IRepository<Doctor>, DoctorRepository>();
            services.AddScoped<IRepository<Visit>, VisitRepository>();
            services.AddScoped<IVisitRepository>, VisitRepository > ();

            // Register Services
            services.AddScoped<PatientService>();
            services.AddScoped<DoctorService>();
            services.AddScoped<VisitService>();

            // Register JwtTokenService
            services.AddSingleton(sp => new JwtTokenService(
                ConfigurationManager.AppSettings["JwtSecretKey"],
                ConfigurationManager.AppSettings["JwtIssuer"],
                ConfigurationManager.AppSettings["JwtAudience"]));

            _serviceProvider = services.BuildServiceProvider();

            // Set dependency resolver for Web API
            GlobalConfiguration.Configuration.DependencyResolver = new MicrosoftWebApiDependencyResolver(_serviceProvider);

            // Set dependency resolver for MVC
            System.Web.Mvc.DependencyResolver.SetResolver(new MicrosoftMvcDependencyResolver(_serviceProvider));
        }

        // Custom dependency resolver for Web API
        private class MicrosoftWebApiDependencyResolver : System.Web.Http.Dependencies.IDependencyResolver
        {
            private readonly IServiceProvider _serviceProvider;

            public MicrosoftWebApiDependencyResolver(IServiceProvider serviceProvider)
            {
                _serviceProvider = serviceProvider;
            }

            public object GetService(Type serviceType)
            {
                return _serviceProvider.GetService(serviceType);
            }

            public System.Collections.Generic.IEnumerable<object> GetServices(Type serviceType)
            {
                return _serviceProvider.GetServices(serviceType);
            }

            public IDependencyScope BeginScope()
            {
                return new MicrosoftWebApiDependencyScope(_serviceProvider.CreateScope());
            }

            public void Dispose()
            {
                // No-op, as IServiceProvider handles disposal
            }
        }

        // Custom dependency scope for Web API
        private class MicrosoftWebApiDependencyScope : IDependencyScope
        {
            private readonly IServiceScope _scope;

            public MicrosoftWebApiDependencyScope(IServiceScope scope)
            {
                _scope = scope;
            }

            public object GetService(Type serviceType)
            {
                return _scope.ServiceProvider.GetService(serviceType);
            }

            public System.Collections.Generic.IEnumerable<object> GetServices(Type serviceType)
            {
                return _scope.ServiceProvider.GetServices(serviceType);
            }

            public void Dispose()
            {
                _scope.Dispose();
            }
        }

        // Custom dependency resolver for MVC
        private class MicrosoftMvcDependencyResolver : System.Web.Mvc.IDependencyResolver
        {
            private readonly IServiceProvider _serviceProvider;

            public MicrosoftMvcDependencyResolver(IServiceProvider serviceProvider)
            {
                _serviceProvider = serviceProvider;
            }

            public object GetService(Type serviceType)
            {
                return _serviceProvider.GetService(serviceType);
            }

            public System.Collections.Generic.IEnumerable<object> GetServices(Type serviceType)
            {
                return _serviceProvider.GetServices(serviceType);
            }
        }
    }
}
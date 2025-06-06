using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using WebActivatorEx;
using Swashbuckle.Application;
using Swashbuckle.Swagger;
using System.Web.Http.Description;

[assembly: WebActivatorEx.PreApplicationStartMethod(typeof(FinalAPI.App_Start.SwaggerConfig), "Register")]

namespace FinalAPI.App_Start
{
    /// <summary>
    /// Configures Swagger for API documentation.
    /// </summary>
    public class SwaggerConfig
    {
        /// <summary>
        /// Registers Swagger configuration for the application.
        /// </summary>
        public static void Register()
        {
            var thisAssembly = typeof(SwaggerConfig).Assembly;

            GlobalConfiguration.Configuration
                .EnableSwagger(c =>
                {
                    c.SingleApiVersion("v1", "Health Billing System API")
                        .Description("API for managing patients, doctors, and visits in a health billing system.")
                        .TermsOfService("None")
                        .Contact(cc => cc
                            .Name("Your Name")
                            .Email("your.email@example.com"));

                    c.IncludeXmlComments(GetXmlCommentsPath());

                    c.ApiKey("Authorization")
                        .Description("Bearer token for JWT Authentication")
                        .Name("Authorization")
                        .In("header");

                    c.OperationFilter<AddAuthorizationHeader>();

                    c.Schemes(new[] { "http", "https" });
                })
                .EnableSwaggerUi(c =>
                {
                    c.EnableApiKeySupport("Authorization", "header");
                });
        }

        private static string GetXmlCommentsPath()
        {
            return System.String.Format(@"{0}\bin\FinalAPI.XML", System.AppDomain.CurrentDomain.BaseDirectory);
        }
    }

    /// <summary>
    /// Operation filter to add Authorization header for endpoints that require authentication.
    /// </summary>
    public class AddAuthorizationHeader : IOperationFilter
    {
        /// <summary>
        /// Applies the Authorization header to Swagger operations that require authentication.
        /// </summary>
        /// <param name="operation">The Swagger operation to modify.</param>
        /// <param name="schemaRegistry">The schema registry for Swagger.</param>
        /// <param name="apiDescription">The API description for the operation.</param>
        public void Apply(Operation operation, SchemaRegistry schemaRegistry, ApiDescription apiDescription)
        {
            var authorizeAttributes = apiDescription
                .ActionDescriptor
                .GetCustomAttributes<AuthorizeAttribute>()
                .Concat(apiDescription.ActionDescriptor.ControllerDescriptor.GetCustomAttributes<AuthorizeAttribute>());

            if (authorizeAttributes.Any())
            {
                if (operation.parameters == null)
                {
                    operation.parameters = new List<Parameter>();
                }

                operation.parameters.Add(new Parameter
                {
                    name = "Authorization",
                    @in = "header",
                    description = "Bearer token",
                    required = true,
                    type = "string"
                });
            }
        }
    }
}
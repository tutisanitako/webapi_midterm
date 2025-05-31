using Swashbuckle.Application;
using System.Web.Http;
using WebActivatorEx;
using Swashbuckle.Swagger;
using System.Collections.Generic;
using System.Web.Http.Description;

[assembly: PreApplicationStartMethod(typeof(Wordle_hw.App_Start.SwaggerConfig), "Register")]

namespace Wordle_hw.App_Start
{
    public class SwaggerConfig
    {
        public static void Register()
        {
            GlobalConfiguration.Configuration
                .EnableSwagger(c =>
                {
                    c.SingleApiVersion("v1", "Wordle_hw");

                    // Add JWT support
                    c.ApiKey("Authorization")
                     .Description("JWT Authorization header using the Bearer scheme. Example: \"Bearer {token}\"")
                     .Name("Authorization")
                     .In("header");

                    c.OperationFilter<AddAuthorizationHeaderParameterOperationFilter>();
                })
                .EnableSwaggerUi(c =>
                {
                    // Optional: make Swagger UI nicer
                });
        }
    }

    public class AddAuthorizationHeaderParameterOperationFilter : IOperationFilter
    {
        public void Apply(Operation operation, SchemaRegistry schemaRegistry, ApiDescription apiDescription)
        {
            if (operation.parameters == null)
                operation.parameters = new List<Parameter>();

            operation.parameters.Add(new Parameter
            {
                name = "Authorization",
                @in = "header",
                description = "Access token (JWT) in format: Bearer {token}",
                required = false,
                type = "string"
            });
        }
    }
}

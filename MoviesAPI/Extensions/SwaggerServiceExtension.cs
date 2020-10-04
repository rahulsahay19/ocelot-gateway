using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;

namespace MoviesAPI.Extensions
{
    public static class SwaggerServiceExtension
    {
        private static ILogger _logger;
        private static IConfiguration _configuration;
        public static IServiceCollection AddSwaggerGenerator(this IServiceCollection services, ILoggerFactory loggerFactory, IConfiguration configuration)
        {
            _logger = loggerFactory.CreateLogger(typeof(SwaggerServiceExtension));
            _configuration = configuration;

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Movies API", Version = "v1" });
            });
            return services;
        }

        public static IApplicationBuilder UseSwaggerBuilder(this IApplicationBuilder app, IApiVersionDescriptionProvider provider)
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.DisplayOperationId();
                c.RoutePrefix = "swagger";

                // build a swagger endpoint for each discovered API version
                foreach (var description in provider.ApiVersionDescriptions.OrderByDescending(d => d.ApiVersion))
                {
                    c.SwaggerEndpoint($"{description.GroupName}/swagger.json", description.GroupName.ToUpperInvariant());
                }
            });
            return app;
        }
    }

}

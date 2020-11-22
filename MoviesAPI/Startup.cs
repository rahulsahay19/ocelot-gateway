using Common.Consul;
using Consul;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MoviesAPI.Extensions;
using IContainer = Autofac.IContainer;

namespace MoviesAPI
{
    public class Startup
    {
        private readonly ILoggerFactory _loggerFactory;
        public IConfiguration _configuration { get; }
        public IContainer Container { get; private set; }
        public Startup(IConfiguration configuration, ILoggerFactory loggerFactory)
        {
            _loggerFactory = loggerFactory;
            _configuration = configuration;
        }
        
        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            //services.AddControllers();
            services.AddMvcCore()
                .SetCompatibilityVersion(CompatibilityVersion.Latest)
                .AddApiExplorer();
            services.AddConsul();

            services.AddVersionedApiExplorer(options =>
            {
                options.GroupNameFormat = "'v'VVV";
                options.SubstituteApiVersionInUrl = true;
            });

            services.AddSwaggerGenerator(_loggerFactory, _configuration);
            services.AddApiVersioning();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IApiVersionDescriptionProvider provider,
            IHostApplicationLifetime applicationLifetime, IConsulClient consulClient)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            //  app.UseHttpsRedirection();
           // app.UseCors(builder => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());

            app.UseRouting();

          //  app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
            app.UseSwaggerBuilder(provider);
            var serviceId = app.UseConsul();
                // Disposing container when application getting stopped.
                applicationLifetime.ApplicationStopped.Register(() =>
                {
                    // Once app becomes offline then remove it from consul. Otherwise it will remain in consul service registry
                    // as dead service
                    consulClient.Agent.ServiceDeregister(serviceId);
                    Container.Dispose();
                });
        }
    }
}

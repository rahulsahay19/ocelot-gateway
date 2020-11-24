using System;
using System.Reflection;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Common;
using Common.App;
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
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {

            //services.AddControllers();
            //services.AddMvcCore()
            //    .SetCompatibilityVersion(CompatibilityVersion.Latest)
            //    .AddApiExplorer();
            services.AddCustomMvc();
            services.AddConsul();
            var builder = new ContainerBuilder();
            //builder.RegisterType<DiscountRepository>().As<IDiscountRepository>();
            //This will register all interfaces alongwith its different implementations
            builder.RegisterAssemblyTypes(Assembly.GetEntryAssembly())
                .AsImplementedInterfaces();
            // This will copy paste services in autofac container. This means whenever it gets registered by asp.net by default at services.AddCustomMvc();
            // this will continue registering new components there.
            

            services.AddVersionedApiExplorer(options =>
            {
                options.GroupNameFormat = "'v'VVV";
                options.SubstituteApiVersionInUrl = true;
            });

            services.AddSwaggerGenerator(_loggerFactory, _configuration);
            services.AddApiVersioning();
            builder.Populate(services);
            Container = builder.Build();
            return new AutofacServiceProvider(Container);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IApiVersionDescriptionProvider provider,
            IHostApplicationLifetime applicationLifetime, IStartupInitializer initializer, IConsulClient consulClient)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            //  app.UseHttpsRedirection();
            //app.UseCors(builder => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());

            initializer.InitializeAsync();
            app.UseRouting();

            //  app.UseAuthorization();
            // app.UseMvc();
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

using System;
using System.Reflection;
using System.Text;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Common;
using Common.App;
using Common.Consul;
using Consul;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Ocelot.Cache.CacheManager;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Ocelot.Provider.Consul;
using Ocelot.Provider.Polly;

namespace OcelotGateway
{
    public class Startup
    {
        private static readonly string[] Headers = new[] { "X-Operation", "X-Resource", "X-Total-Count" };
        public IContainer Container { get; private set; }
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddCustomMvc();
            services.AddAuthentication(option =>
            {
                option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(option =>
            {
                option.RequireHttpsMetadata = true;
                option.SaveToken = true;
                option.TokenValidationParameters = new TokenValidationParameters
                {
                    IssuerSigningKey =
                        new SymmetricSecurityKey(
                            Encoding.ASCII.GetBytes(Configuration.GetSection("JWTSettings:SecretKey").Value)),
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = false,
                    ValidateAudience = false
                };

            });
            services.AddConsul();
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy", cors =>
                    cors.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials()
                        .WithExposedHeaders(Headers));
            });
            services.AddOcelot()
                .AddCacheManager(option => option.WithDictionaryHandle())
                .AddPolly();
            //.AddConsul();

            var builder = new ContainerBuilder();
            builder.RegisterAssemblyTypes(Assembly.GetEntryAssembly())
                .AsImplementedInterfaces();
            builder.Populate(services);
           
            Container = builder.Build();

            return new AutofacServiceProvider(Container);

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IHostApplicationLifetime applicationLifetime, IConsulClient client, IStartupInitializer startupInitializer)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();
            app.UseAuthentication();
            app.UseServiceId();
            
            app.UseEndpoints(endpoints =>
                {
                    endpoints.MapGet("/",
                        async context => { await context.Response.WriteAsync("Ocelot API Gateway"); });
                });
            var consulServiceId = app.UseConsul();
            applicationLifetime.ApplicationStopped.Register(() =>
            {
                client.Agent.ServiceDeregister(consulServiceId);
                Container.Dispose();
            });
            startupInitializer.InitializeAsync();
            app.UseOcelot().Wait();

        }
    }
}

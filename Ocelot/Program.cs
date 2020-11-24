using Common.App;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace OcelotGateway
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            // CreateHostBuilder(args).Build().Run();
            CreateWebHostBuilder(args).Build().Run();
        }

        //public static IHostBuilder CreateHostBuilder(string[] args) =>
        //    Host.CreateDefaultBuilder(args)
        //        .ConfigureWebHostDefaults(webBuilder =>
        //        {
        //            var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
        //            webBuilder.UseStartup<Startup>();
        //            webBuilder.ConfigureAppConfiguration(config => { config.AddJsonFile($"ocelot.{env}.json"); });
        //        }).ConfigureLogging(logging => { logging.AddConsole(); });

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                //   .UseLogging()
                //   .UseVault()
                .UseLockbox();

        //  .UseAppMetrics();
    }
}

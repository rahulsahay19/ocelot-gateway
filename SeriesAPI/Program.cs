using System;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NLog.Web;

namespace SeriesAPI
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var logger = NLogBuilder.ConfigureNLog("nlog.config").GetCurrentClassLogger();
            try
            {
                logger.Debug("Movies.Api starting up...");
                CreateHostBuilder(args).Build().Run();
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Stopped Notifications.Api because of exception: " + ex);
            }
            finally
            {
                NLog.LogManager.Shutdown();
            }
        }

        public static IWebHostBuilder CreateHostBuilder(string[] args)
        {
            var hostingCfg = new ConfigurationBuilder()
                .AddCommandLine(args)
                .Build();

            return WebHost.CreateDefaultBuilder(args)
                .UseConfiguration(hostingCfg)
                .ConfigureLogging(logging =>
                {
                    logging.AddConsole();
                    logging.AddDebug();
                })
                .UseNLog()
                .UseStartup<Startup>();
        }
    }
}

using ConsoleRasp.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using Microsoft.Extensions.Logging;
using NLog;
using NLog.Extensions.Logging;

namespace ConsoleRasp
{
    class Program
    {
        private static ServiceCollection Services { get; set; }
        private static IServiceProvider Provider { get; set; }
        private static Logger logger { get; set; }

        static void Main(string[] args)
        {
            Logger logger = LogManager.LoadConfiguration("nlog.config").GetCurrentClassLogger();
            logger.Info("Initialisation du programme.");
            logger.Info("Creating service collection");
            Services = new ServiceCollection();
            ConfigureServices(Services);

            logger.Info("Building service provider");
            Provider = Services.BuildServiceProvider();

            try
            {
                logger.Info("Starting service");
                Provider.GetService<App>().Run();
                logger.Info("Ending service");
            }
            catch (Exception ex)
            {
                logger.Fatal(ex, "Error running service");
                throw;
            }
            finally
            {
                LogManager.Shutdown();
            }
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", false, true)
                .Build();

            services.AddSingleton(configuration);

            services.Configure<AppConfiguration>(configuration.GetSection("AppConfiguration"));

            // Add app
            services.AddTransient<App>();
        }

        /// <summary>
        /// Configure le Logger.
        /// </summary>
        protected static void ConfigureLogger()
        {
            ILoggerFactory loggerFactory = Provider.GetRequiredService<ILoggerFactory>();

            // Configure NLog
            loggerFactory.AddNLog(new NLogProviderOptions { CaptureMessageTemplates = true, CaptureMessageProperties = true });
            LogManager.LoadConfiguration("nlog.config");

        }
    }
}

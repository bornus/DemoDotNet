using ConsoleRasp.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;

namespace ConsoleRasp
{
    class Program
    {
        private static ServiceCollection Services { get; set; }
        private const int LED_PIN = 18;
        private const int BTN_PIN = 19;

        static void Main(string[] args)
        {
            Services = new ServiceCollection();
            ConfigureServices(Services);

            IServiceProvider serviceProvider = Services.BuildServiceProvider();

            try
            {
                serviceProvider.GetService<App>().Run();
            }
            catch (Exception ex)
            {
                throw;
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
    }
}

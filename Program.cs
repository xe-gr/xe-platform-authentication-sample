using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using XePlatformAuthentication.Helpers;
using XePlatformAuthentication.Models;
using XePlatformAuthentication.Services;
using XePlatformAuthentication.Services.Interfaces;

namespace XePlatformAuthentication
{
    internal class Program
    {
        private static async Task Main()
        {
            AppDomain.CurrentDomain.UnhandledException += UnhandledExceptionTrapper;

            var configuration = SetupConfiguration();

            var services = new ServiceCollection();

            var appSettings = configuration.GetSection("AppSettings").Get<AppSettings>();
            services.AddSingleton(appSettings);

            services.AddSingleton<IJwtClient, JwtClient>();
            services.AddSingleton<IEntriesClient, EntriesClient>();
            services.AddSingleton<ITokenCache, TokenCache>();
            services.AddSingleton<Runner>();

            var container = services.BuildServiceProvider();

            var runner = container.GetService<Runner>();

            await runner.Run();
        }

        private static IConfiguration SetupConfiguration()
        {
            return new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile($"appsettings.{EnvironmentExtensions.GetEnvironment()}.json", false)
                .AddEnvironmentVariables()
                .Build();
        }

        private static void UnhandledExceptionTrapper(object sender, UnhandledExceptionEventArgs e)
        {
            var exception = e.ExceptionObject as Exception;
            
            if (exception is AggregateException aggregateException)
                exception = aggregateException.Flatten();
            
            Console.WriteLine($"Global exception handler caught unexpected error: {exception}");
            Environment.Exit(1);
        }
    }
}

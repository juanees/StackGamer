using Core.Mapper;
using Database;
using Fetcher;
using Fetcher.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog;
using NLog.Extensions.Logging;
using Services;
using Shared;
using Shared.Options;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace ScheduledTask
{
    class Program
    {
        private static ILogger<Program> logger;
        static async Task Main(string[] args)
        {
            try
            {
                //https://www.blinkingcaret.com/2018/02/14/net-core-console-logging/

                #region Dependency Injection and Configuration files
                var serviceCollection = new ServiceCollection();
                IConfiguration configuration = new ConfigurationBuilder()
                   .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                   .AddEnvironmentVariables()
                   .AddCommandLine(args)
                   .Build();
                ConfigureServices(serviceCollection, configuration);
                var serviceProvider = serviceCollection.BuildServiceProvider();

                #endregion

                logger = serviceProvider.GetService<ILogger<Program>>();

                logger.LogInformation($"Starting {Assembly.GetEntryAssembly().GetName().Name}");

                logger.LogInformation("Scrapping data");
                var scraper = serviceProvider.GetService<Scraper>();

                try
                {
                    var categories = await scraper.GetCategoriesAndProducts();
                }
                catch (Exception e)
                {
                    logger.LogError(e, e.Message);
                }


               
            }
            catch (Exception e)
            {
                logger.LogError(e, e.Message);
            }
            finally
            {
                logger.LogInformation($"Exiting {Assembly.GetEntryAssembly().GetName().Name}");
                LogManager.Shutdown();
            }
        }

        private static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<StackGamerOption>(configuration.GetSection("StackGamerOption"));

            var stackGamerOption = new StackGamerOption();
            configuration.Bind("StackGamerOption", stackGamerOption);

            var loggingOption = new LoggingOption();
            configuration.Bind("Logging", loggingOption);

            services
                .AddLogging(logBuilder =>
                {
                    logBuilder.ClearProviders();
                    logBuilder.SetMinimumLevel(loggingOption.LogLevel);
                    logBuilder.AddNLog(configuration);
                })
                .AddMemoryCache()
                .AddTransient<Thief>()
                .AddTransient<StackGameContext>()
                .AddTransient<Scraper>()
                .AddSingleton<ParametersService>()
                .AddHttpClient("stack-gamer", c =>
                {
                    c.BaseAddress = new Uri(stackGamerOption.Urls.BaseUrl);
                    c.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Linux; Android 10) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/86.0.4240.185 Mobile Safari/537.36");
                });
        }
    }
}

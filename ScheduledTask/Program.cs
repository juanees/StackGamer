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
using Shared.Common;
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
                var thief = serviceProvider.GetService<Thief>();

                try
                {
                    var categories = await scraper.GetCategoriesAndProducts();
                    var randomCategory = categories[new Random().Next(0,categories.Count)];
                    var randomProd = randomCategory.Products[new Random().Next(0, randomCategory.Products.Count)];

                    var productInfo = await thief.GetProductById(randomProd.ProductId);
                    var test = 0;
                    
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
                .AddTransient<Scraper>()
                .AddDbContext<StackGameContext>(b=>b.UseSqlite(Constants.RELATIVE_PATH_SQL_LITE_BD))
                .AddTransient<ParametersService>()
                .AddHttpClient(Constants.HTTP_CLIENT_STACK_GAMER, c =>
                {
                    c.BaseAddress = new Uri(stackGamerOption.Urls.BaseUrl);
                    c.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Linux; Android 10) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/86.0.4240.185 Mobile Safari/537.36");
                });
        }
    }
}

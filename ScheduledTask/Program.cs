using Core.Mapper;
using Database;
using Fetcher;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog;
using NLog.Extensions.Logging;
using Shared;
using System;

namespace ScheduledTask
{
    class Program
    {
        private static ILogger<Program> logger;
        static void Main(string[] args)
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

            var idProd = 1001;
            ProductDTO prod = null;

            //TODO:(1)Programar logica de actualización
            if (!(prod is null))
            {
                logger.LogInformation($"The following product was found: { prod.Nombre} - {prod.Codigo}");

                try
                {
                    using var db = serviceProvider.GetService<StackGameContext>();

                    db.Add(prod.MapDtoToDao(idProd));
                    db.SaveChanges();

                    logger.LogInformation($"It seems it worked!");
                }
                catch (DbUpdateException dbe)
                {
                    logger.LogError(dbe, "Error performing operations on the database");

                }
                catch (Exception e)
                {
                    logger.LogError(e, "Error ");
                }
            }
            else
            {
                logger.LogInformation($"No product found for id: {idProd}");
            }
            LogManager.Shutdown();
            Console.ReadLine();
        }

        private static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<StackGamerOption>(configuration.GetSection("StackGamerOption"));

            var stackGamerOption = new StackGamerOption();
            configuration.Bind("StackGamerOption", stackGamerOption);

            var loggingOption = new LoggingOption();
            configuration.Bind("Logging", loggingOption);

            services.AddLogging(logBuilder =>
            {
                logBuilder.ClearProviders();
                logBuilder.SetMinimumLevel(loggingOption.LogLevel);
                logBuilder.AddNLog(configuration);
            })
                .AddTransient<Thief>()
                .AddTransient<StackGameContext>()
                .AddHttpClient("stack-gamer", c =>
                {
                    c.BaseAddress = new Uri(stackGamerOption.Urls.BaseUrl);
                    c.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Linux; Android 10) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/86.0.4240.185 Mobile Safari/537.36");
                });
        }
    }
}

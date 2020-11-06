using ScheduledTask.Mapper;
using Database;
using Fetcher;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Shared;
using System;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace ScheduledTask
{
    class Program
    {
        static async Task Main(string[] args)
        {
            ILogger logger;
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

            //var thief = serviceProvider.GetService<Thief>();
            //var prod = await thief.GetProductById(idProd);
            ProductDTO prod = null;

            //TODO:(1)Programar logica de actualización
            if (!(prod is null))
            {
                logger.LogInformation($"Se encontró el siguien producto: {prod.Nombre} - {prod.Codigo}");

                try
                {
                    using var db = serviceProvider.GetService<StackGameContext>();

                    db.Add(prod.MapDtoToDao());
                    db.SaveChanges();

                    logger.LogInformation($"Parece que funcionó!");
                }
                catch (DbUpdateException dbe)
                {
                    logger.LogError(dbe, "Error realizando operaciones en la base de dato");

                }
                catch (Exception e)
                {
                    logger.LogError(e, "Error ");
                }
            }
            else
            {
                logger.LogInformation($"No se encontró un producto para el id {idProd}");
            }
            Console.ReadLine();
        }

        private static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<StackGamerOption>(configuration.GetSection("StackGamerOption"));

            var stackGamerOption = new StackGamerOption();
            configuration.Bind("StackGamerOption", stackGamerOption);

            var loggingOption = new LoggingOption();
            configuration.Bind("Logging", loggingOption);

            services.AddLogging(configure => configure.AddConsole())
                 .Configure<LoggerFilterOptions>(options => options.MinLevel = loggingOption.LogLevel)                 
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

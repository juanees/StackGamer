using Database;
using Fetcher;
using Fetcher.Errors;
using Fetcher.Errors.ApiFetcher;
using Fetcher.Model.ApiFetcher;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog;
using NLog.Extensions.Logging;
using Services;
using Shared.Common;
using Shared.Options;
using System;
using System.Threading.Tasks;

namespace Core.ScheduledTask
{
    /// <summary>
    /// 
    /// </summary>
    public class FetchAndSaveOrUpdateProducts : ICore
    {
        private ServiceProvider serviceProvider = null;

        private ILogger<FetchAndSaveOrUpdateProducts> logger;

        public FetchAndSaveOrUpdateProducts()
        {
                SetUp();
        }

        public void SetUp(params string[] args)
        {
            #region Dependency Injection and Configuration files
            var serviceCollection = new ServiceCollection();
            IConfiguration configuration = new ConfigurationBuilder()
               .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
               .AddEnvironmentVariables()
               .AddCommandLine(args)
               .Build();
            ConfigureServices(serviceCollection, configuration);
            var _serviceProvider = serviceCollection.BuildServiceProvider();
            logger = _serviceProvider.GetService<ILogger<FetchAndSaveOrUpdateProducts>>();
            Result.Setup(cfg => { cfg.Logger = new ResultConfig.Logger(logger); });
            LogManager.AutoShutdown = true;
            serviceProvider = _serviceProvider;
            #endregion
        }

        private void ConfigureServices(IServiceCollection services, IConfiguration configuration)
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
                .AddTransient<ApiFetcher>()
                .AddTransient<Scraper>()
                .AddDbContext<StackGameContext>(b => b.UseSqlite(Constants.RELATIVE_PATH_SQL_LITE_BD))
                .AddTransient<ParametersService>()
                .AddHttpClient(Constants.HTTP_CLIENT_STACK_GAMER, c =>
                {
                    c.BaseAddress = new Uri(stackGamerOption.Urls.BaseUrl);                    
                    c.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Linux; Android 10) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/86.0.4240.185 Mobile Safari/537.36");
                });
        }

        public ILogger<T> GetLogger<T>()
        {
            return serviceProvider.GetService<ILogger<T>>();
        }

        public async Task<Result> FetchAllCategoriesAndProducts() 
        {
            var prod =await serviceProvider.GetService<ApiFetcher>().GetProductById(1233);

            await serviceProvider.GetService<ApiFetcher>().GetProductById(12);

            var parameter = serviceProvider.GetService<ParametersService>().GetParameterAsync("asdsadas");

            var parameter2 = await serviceProvider.GetService<ParametersService>().GetParameterAsync(Shared.Common.ParametersKeys.CATEGORIES_URL_VALIDATION_REGEX);

            var TIM = prod.HasError<TimeOutError>();
            var TIM2 = prod.HasError<JsonInvalidError>();
            
            
            return Result.Ok();
        }

        ~FetchAndSaveOrUpdateProducts()
        {
            Dispose(false);
        }
        public void Dispose()
        {
            Dispose(true);
        }
        protected void Dispose(bool disposing)
        {
            LogManager.Flush();
        }
    }
}

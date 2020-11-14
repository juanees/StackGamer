using Database;
using Fetcher;
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

namespace Core.ScheduledTask
{
    public class FetchAndSaveOrUpdateProducts : ICore
    {
        private readonly ServiceProvider serviceProvider;

        public FetchAndSaveOrUpdateProducts([System.Runtime.CompilerServices.CallerMemberName] string memberName = "")
        {
                serviceProvider = SetUp();
        }

        ~FetchAndSaveOrUpdateProducts()
        {            
            Dispose(false);
        }

        public ServiceProvider SetUp(params string[] args)
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
            LogManager.AutoShutdown = true;
            return _serviceProvider;
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
                .AddTransient<Thief>()
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

namespace ScheduledTask
{
    using Core;
    using Core.ScheduledTask;
    using FluentResults;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using NLog;
    using NLog.Extensions.Logging;
    using Shared.Common;
    using Shared.Options;
    using System;
    using System.IO;
    using System.Threading.Tasks;

    public class Program
    {
        private static ILogger<Program> Logger { get; set; }
        private static ServiceProvider ServiceProvider { get; set; }
        private const string _prefix = "ScheduledTask_";
        private const string _appsettings = "appsettings.json";

        public static async Task Main(string[] args)
        {
            try
            {
                var host = new HostBuilder()
                    .ConfigureAppConfiguration((hostContext, configApp) =>
                    {
                        configApp.SetBasePath(Directory.GetCurrentDirectory());
                        configApp.AddJsonFile(_appsettings, optional: false, reloadOnChange: true);
                        configApp.AddJsonFile($"appsettings.{hostContext.HostingEnvironment.EnvironmentName}.json", optional: true);
                        configApp.AddEnvironmentVariables(prefix: _prefix);
                        configApp.AddCommandLine(args);
                    })
                    .ConfigureServices((hostContext, services) =>
                    {
                        services
                        .Configure<HostOptions>(option => option.ShutdownTimeout = System.TimeSpan.FromSeconds(20))
                        .AddOptions()
                        .AddMemoryCache()
                        .AddApiFetcher()
                        .AddScraper()
                        .AddParametersService()
                        .AddProductsService()
                        .AddUnitOfWork()
                        .Configure<StackGamerOption>(hostContext.Configuration.GetSection(nameof(StackGamerOption)))
                        //.AddSingleton<IHostedService,Core.ScheduledTask.FetchAndSaveOrUpdateProducts>()
                        .AddTransient<FetchAndSaveOrUpdateProducts>()
                        .AddDatabase(hostContext.Configuration.GetConnectionString("stack-gamer"))
                        .AddHttpClient(Constants.HTTP_CLIENT_STACK_GAMER, hostContext.Configuration.GetSection("StackGamerOption:Urls:BaseUrl").Value);
                    })
                    .ConfigureLogging((hostContext, configLogging) =>
                    {
                        configLogging
                        .AddConsole()
                        .AddConfiguration(hostContext.Configuration.GetSection("Logging"))
                        .AddNLog(hostContext.Configuration);
                    })
                    .UseConsoleLifetime()
                    .Build();

                LogManager.AutoShutdown = true;

                using (var serviceScope = host.Services.CreateScope())
                {
                    var services = serviceScope.ServiceProvider;
                    ILogger<Program> logger = services.GetRequiredService<ILogger<Program>>();
                    Result.Setup(cfg => cfg.Logger = new Core.ResultConfig.Logger(logger));
                    try
                    {
                        var task = services.GetRequiredService<FetchAndSaveOrUpdateProducts>();

                        await task.ExecuteAsync();
                        
                        logger.LogInformation("Success");
                    }
                    catch (Exception ex)
                    {
                        logger?.LogError(ex,"Error Occured");
                    }
                }
                LogManager.Shutdown();

                //host.Run();
            }
            catch (Exception)
            {
                LogManager.Shutdown();
            }
        }
    }
}

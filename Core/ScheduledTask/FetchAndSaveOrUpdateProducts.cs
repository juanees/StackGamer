using FluentResults;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Services;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Core.ScheduledTask
{
    public class FetchAndSaveOrUpdateProducts : IHostedService
    {
        private readonly ILogger<FetchAndSaveOrUpdateProducts> logger;
        private readonly IServiceProvider serviceProvider;
        //private readonly CancellationToken cancellationToken;

        public FetchAndSaveOrUpdateProducts(ILogger<FetchAndSaveOrUpdateProducts> logger, IServiceProvider serviceProvider)
        {
            this.logger = logger;
            this.serviceProvider = serviceProvider;
            Result.Setup(cfg => cfg.Logger = new ResultConfig.Logger(logger));
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            try
            {

                logger.LogInformation("FetchAndSaveOrUpdateProducts is Starting");


                var  productsService = GetAllDependencies();

                //List<Category> cats = productsService.GetAllCategories();


                return;
            }
            catch (OperationCanceledException)
            {

            }
            catch (ObjectDisposedException)
            {

            }
            catch (Exception e)
            {
                logger.LogError(e, e.Message);

                throw;
            }
            finally
            {
                var source = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
                CancellationToken tkn = source.Token;
                tkn.Register(() => { logger.LogInformation("FetchAndSaveOrUpdateProducts is Stopping"); });
                source.Cancel();
                tkn.ThrowIfCancellationRequested();
            }
        }


        public async Task StopAsync(CancellationToken cancellationToken)
        {
            logger.LogInformation("FetchAndSaveOrUpdateProducts is Stopping");

            return;
        }

        private ProductsService GetAllDependencies()
        {
            var productsService = serviceProvider.GetRequiredService<ProductsService>();

            return productsService;
        }

    }
}

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
    public class FetchAndSaveOrUpdateProducts 
    {
        private readonly ILogger<FetchAndSaveOrUpdateProducts> logger;
        private readonly ProductsService productsService;
        

        public FetchAndSaveOrUpdateProducts(ILogger<FetchAndSaveOrUpdateProducts> logger, ProductsService productsService)
        {
            this.logger = logger;
            this.productsService = productsService;
            
        }

        public async Task ExecuteAsync()
        {
            logger.LogInformation("FetchAndSaveOrUpdateProducts is Starting");                      
            //Scrap all the categories and products
            logger.LogInformation("Scrapping all the categories and products");
            (await productsService.UpdateCategoriesAndProductsInformation()).Log();
            logger.LogInformation("FetchAndSaveOrUpdateProducts is Stopping");
            
        }
    }
}

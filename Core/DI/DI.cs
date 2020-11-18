using Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    public static class DI
    {
        public static IServiceCollection AddDatabase(this IServiceCollection serviceCollection, string connectionString) 
        {
            serviceCollection.AddDbContext<StackGameContext>(b => b.UseSqlServer(connectionString));
            return serviceCollection;
        }

        public static IServiceCollection AddHttpClient(this IServiceCollection serviceCollection, string clientName, string baseUrl)
        {
            serviceCollection.AddHttpClient(clientName, c =>
            {
                c.BaseAddress = new Uri(baseUrl);
                c.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Linux; Android 10) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/86.0.4240.185             7.36");
            });
            return serviceCollection;
        }

        public static IServiceCollection AddParametersService(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient<ParametersService>();
            return serviceCollection;
        }

        public static IServiceCollection AddProductsService(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient<ProductsService>();
            return serviceCollection;
        }

        public static IServiceCollection AddUnitOfWork(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient<UnitOfWork>();
            return serviceCollection;
        }

        public static IServiceCollection AddApiFetcher(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient<Fetcher.ApiFetcher>();
            return serviceCollection;
        }

        public static IServiceCollection AddScraper(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient<Fetcher.Scraper>();
            return serviceCollection;
        }
    }
}

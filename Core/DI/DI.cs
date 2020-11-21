using Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using Services;
using System;

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
                c.Timeout = TimeSpan.FromMinutes(1);

            })
                // Retry a specified number of times, using a function to 
                // calculate the duration to wait between retries based on 
                // the current retry attempt (allows for exponential backoff)
                // In this case will wait for
                //  2 ^ 1 = 2 seconds then
                //  2 ^ 2 = 4 seconds then
                //  2 ^ 3 = 8 seconds then
                //  2 ^ 4 = 16 seconds then
                //  2 ^ 5 = 32 seconds
            .AddTransientHttpErrorPolicy(builder => builder.WaitAndRetryAsync(5, retryAttempt =>
            TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))));
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

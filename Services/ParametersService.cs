using Database;
using Database.Model;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Services
{
    public class ParametersService
    {
        private readonly IMemoryCache memoryCache;
        private readonly ILogger<ParametersService> logger;

        private static readonly int EXPIRATION_MINUTES = 15;
        private static readonly string PARAMETERS_KEY = "parameters";

        public ParametersService(IMemoryCache _memoryCache, ILogger<ParametersService> _logger)
        {
            memoryCache = _memoryCache;
            logger = _logger;
            SetParameters();
        }

        public Parameter GetParameter(string key)
        {
            if (!memoryCache.TryGetValue<List<Parameter>>(PARAMETERS_KEY, out var parameters))
            {
                parameters = SetParameters();
            }
            return parameters.FirstOrDefault(p => p.Key == key);
        }

        private List<Parameter> SetParameters()
        {
            MemoryCacheEntryOptions cacheEntryOptions = CreateEntryOptions();
            var stackGameContext = new StackGameContext();

            var parameters = stackGameContext.Parameters.ToList();

            //add cache Item with options of callback
            memoryCache.Set(PARAMETERS_KEY, parameters, cacheEntryOptions);

            logger.LogInformation("Parameters seted");

            return parameters;
        }

        private MemoryCacheEntryOptions CreateEntryOptions()
        {
            // set item with token expiration and callback
            TimeSpan expirationMinutes = System.TimeSpan.FromMinutes(EXPIRATION_MINUTES);
            var expirationTime = DateTime.Now.Add(expirationMinutes);
            var expirationToken = new CancellationChangeToken(
                new CancellationTokenSource(TimeSpan.FromMinutes(EXPIRATION_MINUTES)).Token);

            // Create cache item which executes call back function
            var cacheEntryOptions = new MemoryCacheEntryOptions()
           // Pin to cache.
           .SetPriority(Microsoft.Extensions.Caching.Memory.CacheItemPriority.Normal)
           // Set the actual expiration time
           .SetAbsoluteExpiration(expirationTime)
           // Force eviction to run
           .AddExpirationToken(expirationToken)
           // Add eviction callback
           .RegisterPostEvictionCallback(callback: CacheItemRemoved);
            return cacheEntryOptions;
        }

        private void CacheItemRemoved(object key, object value, EvictionReason reason, object state)
        {
            logger.LogTrace(key + " " + value + " removed from cache due to:" + reason);
            SetParameters();
        }
    }
}

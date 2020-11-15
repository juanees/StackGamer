using Database;
using Database.Model;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using Services.Errors.ParametersService;
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
        private readonly StackGameContext stackGameContext;
        private static readonly int EXPIRATION_MINUTES = 15;
        private static readonly string PARAMETERS_KEY = "parameters";

        public ParametersService(IMemoryCache _memoryCache, ILogger<ParametersService> _logger, StackGameContext _stackGameContext)
        {
            memoryCache = _memoryCache;
            logger = _logger;
            stackGameContext = _stackGameContext;

            SetParameters();
        }
                
        public Result<Parameter> GetParameter(string key)
        {
            try
            {
                if (!memoryCache.TryGetValue<List<Parameter>>(PARAMETERS_KEY, out var parameters))
                {
                    parameters = SetParameters();
                }
                var parameter = parameters.FirstOrDefault(p => p.Key == key);
                return (parameter == default ? Result.Fail(new ParameterNotFoundError($"Parameter not found with key: {key}",key)).Log() : Result.Ok(parameter));
            }
            catch (Exception e)
            {
                return Result.Fail(new FetchingParameterError("Error fetching parameters: "+ e.Message).CausedBy(e)).Log();
            }
        }

        private List<Parameter> SetParameters()
        {
            if (!memoryCache.TryGetValue<List<Parameter>>(PARAMETERS_KEY, out var parameters))
            {
                MemoryCacheEntryOptions cacheEntryOptions = CreateEntryOptions();

                parameters = stackGameContext.Parameters.AsNoTracking().ToList();

                //add cache Item with options of callback
                memoryCache.Set(PARAMETERS_KEY, parameters, cacheEntryOptions);

                logger.LogInformation("Parameters seted");
            }
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
            if (reason != EvictionReason.Replaced)
                SetParameters();
        }
    }
}

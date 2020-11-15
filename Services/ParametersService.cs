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
using System.Threading.Tasks;

namespace Services
{
    public class ParametersService
    {
        private readonly IMemoryCache memoryCache;
        private readonly ILogger<ParametersService> logger;
        private readonly StackGameContext stackGameContext;
        private const int EXPIRATION_MINUTES = 30;
        private const string PARAMETERS_KEY = "parameters";

        public ParametersService(IMemoryCache _memoryCache, ILogger<ParametersService> _logger, StackGameContext _stackGameContext)
        {
            memoryCache = _memoryCache;
            logger = _logger;
            stackGameContext = _stackGameContext;
            //SetParameters();
        }

        /// <summary>
        /// Gets a parameter from cache using the key value
        /// Returns a <see cref="Result"/> wrapping a: <see cref="Parameter"/>. 
        ///If something bad happens (<see cref="ResultBase.IsFailed"/>), you have to use <see cref="ResultBase.HasError(Func{Error, bool})"/> to get the following errors: <see cref="ParameterNotFoundError"/> or <see cref="FetchingParameterError"/>
        /// </summary>       
        /// <param name="id">External product id</param> 
        public async Task<Result<Parameter>> GetParameterAsync(string key)
        {
            try
            {
                var parameters = await
                    memoryCache.GetOrCreateAsync(PARAMETERS_KEY, entry =>
                    {
                        entry.SlidingExpiration = TimeSpan.FromSeconds(3);
                        // set item with token expiration and callback
                        TimeSpan expirationMinutes = System.TimeSpan.FromMinutes(EXPIRATION_MINUTES);
                        var expirationTime = DateTime.Now.Add(expirationMinutes);
                        var expirationToken = new CancellationChangeToken(
                            new CancellationTokenSource(TimeSpan.FromMinutes(EXPIRATION_MINUTES)).Token);                        
                        entry
                       // Pin to cache.
                       .SetPriority(Microsoft.Extensions.Caching.Memory.CacheItemPriority.Normal)
                       // Set the actual expiration time
                       .SetAbsoluteExpiration(expirationTime)
                       // Force eviction to run
                       .AddExpirationToken(expirationToken)
                       // Add eviction callback
                       .RegisterPostEvictionCallback(callback: CacheItemRemoved);
                        return GetParametersFromDb();
                    });
                
                var parameter = parameters.FirstOrDefault(p => p.Key == key);

                return (parameter == default ? Result.Fail(new ParameterNotFoundError($"Parameter not found with key: {key}", key)).Log() : Result.Ok(parameter));
            }
            catch (Exception e)
            {
                return Result.Fail(new FetchingParameterError("Error fetching parameters: " + e.Message).CausedBy(e)).Log();
            }
        }

        private Task<List<Parameter>> GetParametersFromDb()
        {
            return stackGameContext.Parameters.AsNoTracking().ToListAsync();
        }

        //private List<Parameter> SetParameters()
        //{
        //    if (!memoryCache.TryGetValue<List<Parameter>>(PARAMETERS_KEY, out var parameters))
        //    {
        //        MemoryCacheEntryOptions cacheEntryOptions = CreateEntryOptions();

        //        //AsNoTracking to not waste resources as the parameters doesnt needs to be tracked
        //        parameters = stackGameContext.Parameters.AsNoTracking().ToList();

        //        //add cache Item with options of callback
        //        memoryCache.Set(PARAMETERS_KEY, parameters, cacheEntryOptions);

        //        logger.LogInformation("Parameters seted");
        //    }
        //    return parameters;
        //}

        //private MemoryCacheEntryOptions CreateEntryOptions()
        //{
        //    // set item with token expiration and callback
        //    TimeSpan expirationMinutes = System.TimeSpan.FromMinutes(EXPIRATION_MINUTES);
        //    var expirationTime = DateTime.Now.Add(expirationMinutes);
        //    var expirationToken = new CancellationChangeToken(
        //        new CancellationTokenSource(TimeSpan.FromMinutes(EXPIRATION_MINUTES)).Token);

        //    // Create cache item which executes call back function
        //    var cacheEntryOptions = new MemoryCacheEntryOptions()
        //   // Pin to cache.
        //   .SetPriority(Microsoft.Extensions.Caching.Memory.CacheItemPriority.Normal)
        //   // Set the actual expiration time
        //   .SetAbsoluteExpiration(expirationTime)
        //   // Force eviction to run
        //   .AddExpirationToken(expirationToken)
        //   // Add eviction callback
        //   .RegisterPostEvictionCallback(callback: CacheItemRemoved);
        //    return cacheEntryOptions;
        //}

        private void CacheItemRemoved(object key, object value, EvictionReason reason, object state)
        {
            logger.LogTrace(key + " " + value + " removed from cache due to:" + reason);
            //if (reason != EvictionReason.Replaced)
            //    SetParameters();
        }
    }
}

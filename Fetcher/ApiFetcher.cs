using Fetcher.Errors;
using Fetcher.Errors.ApiFetcher;
using Fetcher.Model.ApiFetcher;
using FluentResults;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Shared.Common;
using Shared.Options;
using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;

namespace Fetcher
{
    public class ApiFetcher
    {
        private IHttpClientFactory clientFactory;
        private ILogger logger;
        private IOptions<StackGamerOption> stackGamerOptions;

        public ApiFetcher(IHttpClientFactory _clientFactory, IOptions<StackGamerOption> _stackGamerOptions, ILogger<ApiFetcher> _logger)
        {
            clientFactory = _clientFactory;
            logger = _logger;
            stackGamerOptions = _stackGamerOptions;
        }
        
        public async Task<Result<ApiFetcherProduct>> GetProductById(int id)
        {
            string query = stackGamerOptions.Value.Urls.GetProductByIdUrl + id;

            var httpClient = clientFactory.CreateClient(Constants.HTTP_CLIENT_STACK_GAMER);

            try
            {                
                var prod = await httpClient.GetFromJsonAsync<ApiFetcherProduct>(query);                
                return Result.Ok(prod);
            }
            catch (HttpRequestException e) // Non success
            {
                logger.LogError(e, "An error occurred.");
                return Result.Fail(new Error(e.Message).CausedBy(e)).Log();
            }
            catch (OperationCanceledException e) // Timeout
            {
                logger.LogError(e, "Timeout.");
                return Result.Fail(new TimeOutError(e.Message).CausedBy(e)).Log();
            }
            catch (NotSupportedException e) // When content type is not valid
            {
                logger.LogError(e, "The content type is not supported.");
                return Result.Fail(new ContentTypeNotValidError(e.Message).CausedBy(e)).Log();
            }
            catch (JsonException e) // Invalid JSON
            {
                logger.LogError(e, "Invalid JSON.");
                return Result.Fail(new JsonInvalidError(e.Message).CausedBy(e)).Log();
            }
        }
    }
}

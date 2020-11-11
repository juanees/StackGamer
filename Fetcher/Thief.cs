﻿using Fetcher.Model;
using Fetcher.Model.Thief;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Shared;
using Shared.Options;
using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;

namespace Fetcher
{
    public class Thief
    {
        private IHttpClientFactory clientFactory;
        private ILogger logger;
        private IOptions<StackGamerOption> stackGamerOptions;

        public Thief(IHttpClientFactory _clientFactory, IOptions<StackGamerOption> _stackGamerOptions, ILogger<Thief> _logger)
        {
            clientFactory = _clientFactory;
            logger = _logger;
            stackGamerOptions = _stackGamerOptions;
        }

        public async Task<ProductDTO> GetProductById(int id)
        {
            string query = stackGamerOptions.Value.Urls.GetProductByIdUrl + id;

            var httpClient = clientFactory.CreateClient("stack-gamer");

            try
            {
                return await httpClient.GetFromJsonAsync<ProductDTO>(query);
            }
            catch (HttpRequestException e) // Non success
            {
                logger.LogError(e,"An error occurred.");
            }
            catch (NotSupportedException e) // When content type is not valid
            {
                logger.LogError(e,"The content type is not supported.");
            }
            catch (JsonException e) // Invalid JSON
            {
                logger.LogError(e,"Invalid JSON.");
            }

            return null;


            //using var httpResponse = await httpClient.GetAsync(query, HttpCompletionOption.ResponseHeadersRead);
            //try
            //{
            //    httpResponse.EnsureSuccessStatusCode(); // throws if not 200-299
            //}
            //catch (Exception e)
            //{
            //    logger.LogError(0, e, $"Error while making request to get product {id}", id);
            //    return null;
            //}

            //if (httpResponse.Content is object && httpResponse.Content.Headers.ContentType.MediaType == "text/html")
            //{
            //    var contentStream = await httpResponse.Content.ReadAsStreamAsync();

            //    try
            //    {
            //        return await JsonSerializer.DeserializeAsync<ProductDTO>(contentStream, new JsonSerializerOptions { IgnoreNullValues = true, PropertyNameCaseInsensitive = true });

            //    }
            //    catch (JsonException e) // Invalid JSON
            //    {
            //        logger.LogError(e, "Invalid JSON.");
            //    }
            //}
            //else
            //{
            //    logger.LogError("HTTP Response was invalid and cannot be deserialised.");
            //}

            //return null;

        }
    }
}

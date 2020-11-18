using Fetcher.Model.Scraper;
using FluentResults;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using PuppeteerSharp;
using Services;
using Shared.Common;
using Shared.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Fetcher
{
    public class Scraper
    {
        private readonly ILogger logger;
        private readonly IOptions<StackGamerOption> stackGamerOptions;
        private readonly ParametersService parametersService;

        private string CATEGORIES_URL_VALIDATION_REGEX;
        private string PRODUCT_ID_FROM_URL_REGEX;

        public Scraper(IOptions<StackGamerOption> _stackGamerOptions, ILogger<Scraper> _logger, ParametersService _parametersService)
        {
            logger = _logger;
            stackGamerOptions = _stackGamerOptions;
            parametersService = _parametersService;
        }

        public async Task<Result<List<ScraperCategory>>> GetCategoriesAndProducts()
        {
            Result<Database.Model.Parameter> resultParam = await parametersService.GetParameterAsync(ParametersKeys.CATEGORIES_URL_VALIDATION_REGEX);
            if (resultParam.IsFailed) throw new Exception(resultParam.Errors.Join());
            else CATEGORIES_URL_VALIDATION_REGEX = resultParam.Value.Value;

            logger.LogTrace("CATEGORIES_URL_VALIDATION_REGEX: " + CATEGORIES_URL_VALIDATION_REGEX);

            resultParam = await parametersService.GetParameterAsync(ParametersKeys.PRODUCT_ID_FROM_URL_REGEX);
            if (resultParam.IsFailed) throw new Exception(resultParam.Errors.Join());
            else PRODUCT_ID_FROM_URL_REGEX = resultParam.Value.Value;

            logger.LogTrace("PRODUCT_ID_FROM_URL_REGEX: " + PRODUCT_ID_FROM_URL_REGEX);

            logger.LogInformation("Parameters fetched");

            List<ScraperCategory> categories = new List<ScraperCategory>();
            Result result = new Result();
            try
            {
                logger.LogInformation("Scraping web..");
                logger.LogInformation("Downloading browser if necessary");
                await new BrowserFetcher().DownloadAsync(BrowserFetcher.DefaultRevision);
                logger.LogInformation("Scraping started");
                using var browser = await Puppeteer.LaunchAsync(new LaunchOptions
                {
                    Headless = true
                });
                using var page = await browser.NewPageAsync();

                await page.GoToAsync(stackGamerOptions.Value.Urls.CategoriesUrl);

                var modalNotificationSelector = "#aceptarNotificacion";
                await page.WaitForSelectorAsync(modalNotificationSelector);
                await page.ClickAsync(modalNotificationSelector);

                var resultsSelector = "#contenidoCategorias";
                var categoriesSelector = "#contenidoCategorias > ul > li";
                var linkSelector = "ul > li > a";
                await page.WaitForSelectorAsync(resultsSelector);

                var scrappedCategories = await page.EvaluateFunctionAsync(@"(categoriesSelector,linkSelector) => {
                var elList = document.querySelectorAll(categoriesSelector)
                elList = Array.prototype.slice.call(elList, 0);
                var links = new Array();
                for (var el of elList)
                {
                    var lkns = el.querySelectorAll(linkSelector);
                    for (var link of lkns)
                    {
                        if (link.href !== undefined && link.href !== 'javascript:void(0);')
                        {
                            links.push(link)
                        }
                    }
                }
                return links.map(a => { return {url:a.href,desc:a.innerText}});
                }", categoriesSelector, linkSelector);

                var urlValidation = new Regex(CATEGORIES_URL_VALIDATION_REGEX);
                foreach (var scrappedCat in scrappedCategories)
                {
                    try
                    {
                        var scrappedCategory = scrappedCat.ToObject<ScrappedCategory>();
                        var validation = urlValidation.Match(scrappedCategory.Url);
                        if (validation.Success)
                        {
                            if (validation.Groups.Count > 0 && int.TryParse(validation.Groups[1].Value, out int categoryId))
                            {
                                var cat = new ScraperCategory() { CategoryId = categoryId, Description = scrappedCategory.Description.Trim(), Url = new Uri(scrappedCategory.Url) };
                                categories.Add(cat);
                                logger.LogTrace("Category scrapped: {0} - {1}", cat.CategoryId, cat.Description);
                            }
                            else
                                logger.LogWarning("Link not valid: {0}", scrappedCat);
                        }
                        else
                        {
                            logger.LogWarning("Link not valid: {0}", scrappedCat);
                        }
                    }
                    catch (Exception e)
                    {
                        logger.LogError(e, "Error parsing JSON from category");
                        result = Result.Merge(result, Result.Fail(new Error("Error parsing JSON from category").CausedBy(e)));
                    }
                }

                logger.LogTrace("********************************************************************************************");

                categories = categories.OrderBy(x => x.CategoryId).ToList();
                foreach (var cat in categories)
                {

                    logger.LogTrace(">>>>>>>>>>> Category id: {0} - Description: {1} - Url: {2} ", cat.CategoryId, cat.Description, cat.Url);

                    Task.WaitAll(Delay());

                    await page.GoToAsync(cat.Url.ToString());
                    var productsSelector = @"#wrapper > div.border-main > div > div > div.col-md-9.col-sm-9.col-xs-12 > div > ul > li > div > h4 > a";
                    var scrappedProducts = await page.EvaluateFunctionAsync(@"(productsSelector) => { return Array.from(document.querySelectorAll(productsSelector)).map(prod=>{return {url:prod.href,name:prod.innerText}}); }", productsSelector);

                    var productValidation = new Regex(PRODUCT_ID_FROM_URL_REGEX);
                    foreach (var scrappedProd in scrappedProducts)
                    {
                        try
                        {
                            var scrappedProduct = scrappedProd.ToObject<ScrappedProduct>();
                            var validation = productValidation.Match(scrappedProduct.Url);
                            if (validation.Success)
                            {
                                if (validation.Groups.Count > 0 && int.TryParse(validation.Groups[1].Value, out int productId))
                                {
                                    var prod = new Model.Scraper.ScraperProduct() { ProductId = productId, Name = scrappedProduct.Name.Trim(), Url = new Uri(scrappedProduct.Url) };
                                    cat.Products.Add(prod);
                                    logger.LogTrace("Product scrapped: {0} - {1}", prod.ProductId, prod.Name);
                                }
                                else
                                    logger.LogWarning("Product not valid: {0}", scrappedProd);
                            }
                            else
                            {
                                logger.LogWarning("Product not valid: {0}", scrappedProd);
                            }
                        }
                        catch (Exception e)
                        {
                            logger.LogError(e, "Error parsing JSON from product");
                            result = Result.Merge(result, Result.Fail(new Error("Error parsing JSON from product").CausedBy(e)));
                        }
                    }
                }
                logger.LogTrace("********************************************************************************************");
            }
            catch (Exception e)
            {
                logger.LogError(e, "Error scraping {0}", stackGamerOptions.Value.Urls.CategoriesUrl);
                categories = new List<ScraperCategory>();
            }

            logger.LogInformation("{0} categories scrapped", categories.Count);
            logger.LogInformation("{0} products scrapped", categories.Sum(x => x.Products.Count));
            logger.LogInformation("Scraping finished");
            
            if (result.IsFailed)
                return result;

            return Result.Ok(categories);
        }

        private async Task Delay()
        {
            Random rnd = new Random();
            var ms = rnd.Next(500, 501);
            TimeSpan waitTime = new TimeSpan(0, 0, 0, 0, ms);
            logger.LogTrace("Wating {0} seconds between requests", waitTime.TotalSeconds);
            await Task.Delay(Convert.ToInt32(waitTime.TotalMilliseconds));
        }

        #region Support Classes
        private class ScrappedCategory
        {
            [JsonProperty("desc")]
            public string Description { get; set; }

            [JsonProperty("url")]
            public string Url { get; set; }
        }
        private class ScrappedProduct
        {
            [JsonProperty("url")]
            public string Url { get; set; }

            [JsonProperty("name")]
            public string Name { get; set; }
        }
        #endregion
    }
}

using Database;
using Database.Model;
using Fetcher;
using Fetcher.Model.ApiFetcher;
using Fetcher.Model.Scraper;
using FluentResults;
using Microsoft.Extensions.Logging;
using Shared.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Services
{
    public class ProductsService
    {
        private readonly UnitOfWork unitOfWork;
        private readonly Scraper scraper;
        private readonly ILogger<ProductsService> logger;
        private readonly ParametersService parametersService;
        private readonly ApiFetcher apiFetcher;

        public ProductsService(UnitOfWork unitOfWork, Scraper scraper, ILogger<ProductsService> logger, ParametersService parametersService, ApiFetcher apiFetcher)
        {
            this.unitOfWork = unitOfWork;
            this.scraper = scraper;
            this.logger = logger;
            this.parametersService = parametersService;
            this.apiFetcher = apiFetcher;
        }

        public async Task<Result> ScrapAllCategoriesAndProductsAsync()
        {
            var currentTry = 0;
            var productApi = new ApiFetcherProduct();
            int TimeBetweenQueries;
            int MaxQueriesPerProduct;
            Result<Database.Model.Parameter> resultParam;
            try
            {
                Result<List<ScraperCategory>> categoriesResult = await scraper.GetCategoriesAndProducts();
                if (categoriesResult.IsFailed)
                    return categoriesResult.ToResult();

                #region Parameters
                resultParam = await parametersService.GetParameterAsync(ParametersKeys.TIME_BETWEEN_QUERIES);
                if (resultParam.IsFailed) throw new Exception(resultParam.Errors.Join());
                else TimeBetweenQueries = Convert.ToInt32(resultParam.Value.Value);
                logger.LogInformation("TimeBetweenQueries: " + TimeBetweenQueries);

                resultParam = await parametersService.GetParameterAsync(ParametersKeys.MAX_QUERIES_PER_PRODUCT);
                if (resultParam.IsFailed) throw new Exception(resultParam.Errors.Join());
                else MaxQueriesPerProduct = Convert.ToInt32(resultParam.Value.Value);
                logger.LogInformation("MaxQueriesPerProduct: " + MaxQueriesPerProduct);
                #endregion Parameters

                var categories = categoriesResult.Value;
                foreach (var scrapedCategory in categories)
                {
                    logger.LogInformation("Getting product from scraped category {0}", scrapedCategory.Description);
                    var category = unitOfWork.CategoryRepository.Get(c => c.ExternalCategoryId == scrapedCategory.CategoryId, null, nameof(Category.Products)).FirstOrDefault();

                    if (category is null)
                    {
                        category = new Category()
                        {
                            Name = scrapedCategory.Description,
                            ExternalCategoryId = scrapedCategory.CategoryId,
                            Products = new List<Product>()
                        };
                        unitOfWork.CategoryRepository.Insert(category);
                    }
                    else
                    {
                        category.Name = scrapedCategory.Description;
                        category.ExternalCategoryId = scrapedCategory.CategoryId;
                        unitOfWork.CategoryRepository.Update(category);
                    }

                    unitOfWork.Save();

                    foreach (var scrapedProduct in scrapedCategory.Products)
                    {
                        logger.LogInformation("Saving {0}", scrapedProduct.Name);
                        currentTry = 0;
                        logger.LogDebug("currentTry: " + currentTry);
                        while (currentTry < MaxQueriesPerProduct)
                        {
                            var productApiResult = await apiFetcher.GetProductById(scrapedProduct.ProductId);
                            productApiResult.Log();
                            if (productApiResult.IsFailed)
                            {
                                logger.LogError(string.Format("Could not get product {0} from api on iteration {1}", scrapedProduct.Name, currentTry + 1));
                                Thread.Sleep(new TimeSpan(0, 0, TimeBetweenQueries));
                                currentTry++;
                                logger.LogDebug("currentTry: " + currentTry);
                                continue;
                            }

                            productApi = productApiResult.Value;
                            logger.LogInformation("productApi: {0}", productApi.Name);

                            var prod = category.Products.FirstOrDefault(x => x.ExternalProductId == scrapedProduct.ProductId);
                            if (prod is null)
                            {
                                prod = new Product()
                                {
                                    Name = productApi.Name,
                                    ExternalProductId = scrapedProduct.ProductId,
                                    Category = category,
                                    CategoryId = category.Id,
                                    BrandId = productApi.BrandId,
                                    Saleable = (productApi.Saleable ?? 0) == 1,
                                    Code = productApi.Code,
                                    Prices = new List<ProductPrice>()
                                };
                                category.Products.Add(prod);
                            }
                            else
                            {
                                prod.Name = productApi.Name;
                                prod.ExternalProductId = scrapedProduct.ProductId;
                                prod.Category = category;
                                prod.CategoryId = category.Id;
                                prod.BrandId = productApi.BrandId;
                                prod.Saleable = (productApi.Saleable ?? 0) == 1;
                                prod.Code = productApi.Code;
                            }

                            prod.Prices.Add(new ProductPrice()
                            {
                                ListPrice = (productApi.ListPrice ?? 0),
                                PreviousListPrice = (productApi.PreviousListPrice ?? 0),
                                PreviousSpecialPrice = (productApi.PreviousSpecialPrice ?? 0),
                                SpecialPrice = (productApi.SpecialPrice ?? 0),
                                Product = prod
                            });

                            unitOfWork.CategoryRepository.Update(category);
                            unitOfWork.Save();
                            currentTry = MaxQueriesPerProduct;
                        }
                    }
                }

                return Result.Ok();
            }
            catch (Exception e)
            {
                logger.LogError(e, e.Message);
                return Result.Fail(new Error(e.Message).CausedBy(e)).Log();
            }
            finally
            {
                unitOfWork?.Dispose();
            }
        }

        public Task<Result<List<Category>>> FetchPriceForProducts(List<Category> value)
        {
            throw new NotImplementedException();
        }

        public Task SaveOrUpdateProducts(List<Category> value)
        {
            throw new NotImplementedException();
        }
    }
}

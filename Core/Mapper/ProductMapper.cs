using Database.Model;
using Fetcher.Model.ApiFetcher;

namespace Core.Mapper
{
    public static class ProductMapper
    {
        /// <summary>
        /// Extension method to convert from the product fetched from the API (<see cref="ApiFetcherProduct"/>) to the database model Product (<see cref="Product"/>)
        /// </summary>
        /// <param name="product"></param>
        /// <param name="prodCode"></param>
        /// <returns></returns>
        public static Product ConvertToDatabaseProduct(this ApiFetcherProduct product, int prodCode)
        {
            return new()
            {
                Name = product.Name,
                ExternalProductId = prodCode,
                Saleable = product.Saleable == 1,
                Code = product.Code,
                BrandId = product.BrandId,
                CategoryId = product.SubCategoryId,
                SpecialPrice = (product.SpecialPrice ?? 0) / 100,
                PreviousSpecialPrice = (product.PreviousSpecialPrice ?? 0) / 100,
                ListPrice = (product.ListPrice ?? 0) / 100,
                PreviousListPrice = (product.PreviousListPrice ?? 0) / 100
            };
        }
    }
}

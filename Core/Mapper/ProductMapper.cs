using Database.Model;
using Fetcher.Model.ApiFetcher;

namespace Core.Mapper
{
    public static class ProductMapper
    {
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

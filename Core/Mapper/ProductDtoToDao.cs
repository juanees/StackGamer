using Database;
using Database.Model;
using Fetcher.Model;
using Fetcher.Model.Thief;

namespace Core.Mapper
{
    public static class ProductDtoToDao
    {
        public static ProductDAO MapDtoToDao(this ProductDTO productDTO, int prodCode)
        {
            var prod = new ProductDAO()
            {
                Name = productDTO.Name,
                ExternalIdProduct = prodCode,
                Salable = productDTO.Salable == 1,
                Code = productDTO.Code,
                BrandId = productDTO.IdBrand,
                CategoryId = productDTO.IdSubCategory,
                SpecialPrice = (productDTO.SpecialPrice ?? 0) / 100,
                PreviousSpecialPrice = (productDTO.PreviousSpecialPrice ?? 0) / 100,
                ListPrice = (productDTO.ListPrice ?? 0) / 100,
                PreviousListPrice = (productDTO.PreviousListPrice ?? 0) / 100
            };
            return prod;
        }
    }
}

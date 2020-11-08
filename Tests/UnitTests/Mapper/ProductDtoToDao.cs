using Core.Mapper;
using Fetcher;
using Fetcher.Model;
using Fetcher.Model.Thief;
using NUnit.Framework;

namespace Tests.UnitTests.Mapper
{

    [TestFixture]
    public class ProductDtoToDao
    {
        private ProductDTO _productDTO;

        [SetUp]
        public void SetUp()
        {
            _productDTO = new ProductDTO()
            {
                Name = "  test TEST ñ 🤡 ",
                Salable = null,
                Code = "COD 1",
                IdCategory = 1,
                IdBrand = 2,
                IdSubCategory = 3,
                SpecialPrice = 100,
                PreviousSpecialPrice = 200,
                ListPrice = null,
                PreviousListPrice = 0
            };
        }

        [Test]
        public void MapDtoToDao_IsWorkingCorrectly()
        {
            int codeProd = int.MaxValue;
            var result = _productDTO.MapDtoToDao(codeProd);

            Assert.AreEqual(result.Name, _productDTO.Name);

            Assert.AreEqual(result.ExternalIdProduct, codeProd);

            Assert.IsFalse(result.Salable);

            Assert.AreEqual(result.Code, _productDTO.Code);

            Assert.AreEqual(result.BrandId, _productDTO.IdBrand);

            Assert.AreEqual(result.CategoryId, _productDTO.IdSubCategory);

            Assert.AreEqual(result.SpecialPrice * 100, _productDTO.SpecialPrice);

            Assert.AreEqual(result.PreviousSpecialPrice * 100, _productDTO.PreviousSpecialPrice);

            Assert.AreEqual(result.ListPrice, 0);

            Assert.AreEqual(result.PreviousListPrice * 100, _productDTO.PreviousListPrice);
        }
    }
}

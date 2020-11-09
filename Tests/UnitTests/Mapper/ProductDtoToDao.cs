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
                Saleable = null,
                Code = "COD 1",
                CategoryId = 1,
                BrandId = 2,
                SubCategoryId = 3,
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

            Assert.AreEqual(result.ExternalProductId, codeProd);

            Assert.IsFalse(result.Saleable);

            Assert.AreEqual(result.Code, _productDTO.Code);

            Assert.AreEqual(result.BrandId, _productDTO.BrandId);

            Assert.AreEqual(result.CategoryId, _productDTO.SubCategoryId);

            Assert.AreEqual(result.SpecialPrice * 100, _productDTO.SpecialPrice);

            Assert.AreEqual(result.PreviousSpecialPrice * 100, _productDTO.PreviousSpecialPrice);

            Assert.AreEqual(result.ListPrice, 0);

            Assert.AreEqual(result.PreviousListPrice * 100, _productDTO.PreviousListPrice);
        }
    }
}

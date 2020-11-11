using Core.Mapper;
using Fetcher;
using Fetcher.Model;
using Fetcher.Model.Thief;
using NUnit.Framework;

namespace Tests.UnitTests.Mapper
{

    [TestFixture]
    public class ConvertToDatabaseProduct
    {
        private Product _product;

        [SetUp]
        public void SetUp()
        {
            _product = new Product()
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
        public void ConvertToDatabaseProduct_IsWorkingCorrectly()
        {
            int codeProd = int.MaxValue;
            var result = _product.ConvertToDatabaseProduct(codeProd);

            Assert.AreEqual(result.Name, _product.Name);

            Assert.AreEqual(result.ExternalProductId, codeProd);

            Assert.IsFalse(result.Saleable);

            Assert.AreEqual(result.Code, _product.Code);

            Assert.AreEqual(result.BrandId, _product.BrandId);

            Assert.AreEqual(result.CategoryId, _product.SubCategoryId);

            Assert.AreEqual(result.SpecialPrice * 100, _product.SpecialPrice);

            Assert.AreEqual(result.PreviousSpecialPrice * 100, _product.PreviousSpecialPrice);

            Assert.AreEqual(result.ListPrice, 0);

            Assert.AreEqual(result.PreviousListPrice * 100, _product.PreviousListPrice);
        }
    }
}

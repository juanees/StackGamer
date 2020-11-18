using Fetcher.Errors.ApiFetcher;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Protected;
using NUnit.Framework;
using Shared.Common;
using Shared.Options;
using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Tests.IntegrationTests.Fetcher
{
    [TestFixture]
    public class ApiFetcherTest
    {
        private IOptions<StackGamerOption> someOptions;

        [SetUp]
        public void SetUp()
        {
            someOptions = Options.Create(new StackGamerOption()
            {
                Urls = new StackGamerOption.UrlsOption()
                {
                    BaseUrl = "https://test.com",
                    GetProductByIdUrl = "/test/get_product_by_id_url?id_producto="
                }
            });
        }

        //TODO (1) ADD Fetcher_ApiFetcher_Product_Should_Be_Null_When_Content_Type_Not_Valid
        //TODO (2) ADD Fetcher_ApiFetcher_Product_Should_Be_Null_When_Time_Out

        [Test]
        public async Task Fetcher_ApiFetcher_Product_Should_Not_Be_Null_When_Http_Status_Code_200()
        {
            //https://www.thecodebuzz.com/unit-test-mock-httpclientfactory-moq-net-core/
            //https://stackoverflow.com/questions/56064031/mocking-httpclient-getasync-by-using-moq-library-in-xunit-test
            string json = "{\"coins\":0,\"coins_forzada\":null,\"coins_sale\":null,\"destacado\":0,\"nombre\":\"Parlantes Edifier X100 2.1 \",\"id_categoria\":5,\"id_subcategoria\":65,\"garantia\":\"6 meses\",\"id_marca\":130,\"precioEspecial\":5409,\"precioLista\":5778,\"precioListaAnterior\":null,\"precioEspecialAnterior\":null,\"imagenes\":null,\"vendible\":0,\"codigo\":\"SIN STOCK\",\"niveles\":null,\"reviews\":null,\"tv\":null,\"sale\":null,\"incluidos\":null}";
            var status = HttpStatusCode.OK;            

            var _loggerMock = Mocks.MocksCreator.CreateLoggerMock<global::Fetcher.ApiFetcher>();
            var _httpFactory = Mocks.MocksCreator.CreateHttpClientFactory(status, json, someOptions.Value.Urls.BaseUrl,Constants.HTTP_CLIENT_STACK_GAMER);

            var _ApiFetcher = new global::Fetcher.ApiFetcher(_httpFactory.Object, someOptions, _loggerMock.Object);

            var productId = 1000;
            var result = await _ApiFetcher.GetProductById(productId);
            
            var retrivedProduct = result.Value;

            //Assert
            Assert.NotNull(result);            
            Assert.IsTrue(result.IsSuccess);
            Assert.IsFalse(result.IsFailed);

            Assert.NotNull(result.Value);            

            Assert.NotNull(retrivedProduct);

            Assert.AreEqual(retrivedProduct.Name, "Parlantes Edifier X100 2.1 ");

            Assert.IsNotNull(retrivedProduct.Saleable);

            Assert.AreEqual(retrivedProduct.Saleable, 0);

            Assert.AreEqual(retrivedProduct.Code, "SIN STOCK");

            Assert.AreEqual(retrivedProduct.CategoryId, 5);

            Assert.AreEqual(retrivedProduct.BrandId, 130);

            Assert.AreEqual(retrivedProduct.SubCategoryId, 65);

            Assert.AreEqual(retrivedProduct.SpecialPrice , 5409);

            Assert.IsNull(retrivedProduct.PreviousSpecialPrice);

            Assert.AreEqual(retrivedProduct.PreviousSpecialPrice , null);

            Assert.AreEqual(retrivedProduct.ListPrice, 5778);

            Assert.IsNull(retrivedProduct.PreviousListPrice);

            Assert.AreEqual(retrivedProduct.PreviousListPrice , null);

        }

        [Test]
        public async Task Fetcher_ApiFetcher_Product_Should_Be_Null_When_Json_Error()
        {

            //https://www.thecodebuzz.com/unit-test-mock-httpclientfactory-moq-net-core/
            //https://stackoverflow.com/questions/56064031/mocking-httpclient-getasync-by-using-moq-library-in-xunit-test
            string json = "{\"coins\":0,\"coins_forzada:null,\"coins_sale\":null,\"destacado\":0,\"nombre\":\"Parlantes Edifier X100 2.1 \",\"id_categoria\":5,\"id_subcategoria\":65,\"garantia\":\"6 meses\",\"id_marca\":130,\"precioEspecial\":5409,\"precioLista\":5778,\"precioListaAnterior\":null,\"precioEspecialAnterior\":null,\"imagenes\":null,\"vendible\":0,\"codigo\":\"SIN STOCK\",\"niveles\":null,\"reviews\":null,\"tv\":null,\"sale\":null,\"incluidos\":null}";
            var status = HttpStatusCode.OK;

            var _loggerMock = Mocks.MocksCreator.CreateLoggerMock<global::Fetcher.ApiFetcher>();
            var _httpFactory = Mocks.MocksCreator.CreateHttpClientFactory(status, json, someOptions.Value.Urls.BaseUrl, Constants.HTTP_CLIENT_STACK_GAMER);

            var _ApiFetcher = new global::Fetcher.ApiFetcher(_httpFactory.Object, someOptions, _loggerMock.Object);

            var productId = 1000;
            var result = await _ApiFetcher.GetProductById(productId);

            _loggerMock.Verify(
                x => x.Log(
                    It.Is<LogLevel>(l => l == LogLevel.Error),
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => true),
                    It.IsAny<Exception>(),
                    It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)));

            //Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.IsFailed);
            Assert.False(result.IsSuccess);
            Assert.Throws<InvalidOperationException>(delegate { var _ = result.Value; });

            Assert.IsTrue(result.HasError<JsonInvalidError>());
            JsonInvalidError error = (JsonInvalidError)result.Errors.Find(x => x.GetType() == typeof(JsonInvalidError));
            Assert.IsNotNull(error);
        }

        [Test]
        public async Task Fetcher_ApiFetcher_Product_Should_Be_Null_When_Http_Status_Code_Not_200()
        {
            //https://www.thecodebuzz.com/unit-test-mock-httpclientfactory-moq-net-core/
            //https://stackoverflow.com/questions/56064031/mocking-httpclient-getasync-by-using-moq-library-in-xunit-test
            string json = "{\"coins\":0,\"coins_forzada\":null,\"coins_sale\":null,\"destacado\":0,\"nombre\":\"Parlantes Edifier X100 2.1 \",\"id_categoria\":5,\"id_subcategoria\":65,\"garantia\":\"6 meses\",\"id_marca\":130,\"precioEspecial\":5409,\"precioLista\":5778,\"precioListaAnterior\":null,\"precioEspecialAnterior\":null,\"imagenes\":null,\"vendible\":0,\"codigo\":\"SIN STOCK\",\"niveles\":null,\"reviews\":null,\"tv\":null,\"sale\":null,\"incluidos\":null}";
            var status = HttpStatusCode.NotFound;

            var _loggerMock = Mocks.MocksCreator.CreateLoggerMock<global::Fetcher.ApiFetcher>();
            var _httpFactory = Mocks.MocksCreator.CreateHttpClientFactory(status, json, someOptions.Value.Urls.BaseUrl, Constants.HTTP_CLIENT_STACK_GAMER);

            var _ApiFetcher = new global::Fetcher.ApiFetcher(_httpFactory.Object, someOptions, _loggerMock.Object);

            var productId = 1000;
            var result = await _ApiFetcher.GetProductById(productId);

            _loggerMock.Verify(
                x => x.Log(
                    It.Is<LogLevel>(l => l == LogLevel.Error),
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => true),
                    It.IsAny<Exception>(),
                    It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)));

            //Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.IsFailed);
            Assert.IsFalse(result.IsSuccess);
            Assert.Throws<InvalidOperationException>(delegate { var _ = result.Value; });
        }
    }
}
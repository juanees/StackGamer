using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Protected;
using NUnit.Framework;
using Shared.Options;
using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Tests.UnitTests.Fetcher
{
    [TestFixture]
    public class Thief
    {

        private IOptions<StackGamerOption> someOptions;



        [SetUp]
        public void SetUp()
        {
            someOptions = Options.Create(new StackGamerOption()
            {
                Urls = new Urls()
                {
                    BaseUrl = "https://test.com",
                    GetProductByIdUrl = "/test/get_product_by_id_url?id_producto="
                }
            });
        }

        [Test]
        public async Task Fetcher_Thief_IsWorkingCorrectly()
        {

            //https://www.thecodebuzz.com/unit-test-mock-httpclientfactory-moq-net-core/
            //https://stackoverflow.com/questions/56064031/mocking-httpclient-getasync-by-using-moq-library-in-xunit-test
            string json = "{\"coins\":0,\"coins_forzada\":null,\"coins_sale\":null,\"destacado\":0,\"nombre\":\"Parlantes Edifier X100 2.1 \",\"id_categoria\":5,\"id_subcategoria\":65,\"garantia\":\"6 meses\",\"id_marca\":130,\"precioEspecial\":5409,\"precioLista\":5778,\"precioListaAnterior\":null,\"precioEspecialAnterior\":null,\"imagenes\":null,\"vendible\":0,\"codigo\":\"SIN STOCK\",\"niveles\":null,\"reviews\":null,\"tv\":null,\"sale\":null,\"incluidos\":null}";
            var status = HttpStatusCode.OK;
            CreateMocks(out _, out global::Fetcher.Thief _thief, out _, json, status);

            var productId = 1000;
            var result = await _thief.GetProductById(productId);

            //Assert
            Assert.NotNull(result);

            Assert.AreEqual(result.Name, "Parlantes Edifier X100 2.1 ");

            Assert.IsNotNull(result.Saleable);

            Assert.AreEqual(result.Saleable, 0);

            Assert.AreEqual(result.Code, "SIN STOCK");

            Assert.AreEqual(result.CategoryId, 5);

            Assert.AreEqual(result.BrandId, 130);

            Assert.AreEqual(result.SubCategoryId, 65);

            Assert.AreEqual(result.SpecialPrice , 5409);

            Assert.IsNull(result.PreviousSpecialPrice);

            Assert.AreEqual(result.PreviousSpecialPrice , null);

            Assert.AreEqual(result.ListPrice, 5778);

            Assert.IsNull(result.PreviousListPrice);

            Assert.AreEqual(result.PreviousListPrice , null);

        }

        private void CreateMocks(out Mock<ILogger<global::Fetcher.Thief>> _loggerMock, out global::Fetcher.Thief _thief, out Mock<IHttpClientFactory> _mockFactory, string json, HttpStatusCode status)
        {
            _mockFactory = new Mock<IHttpClientFactory>();
            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = status,
                    Content = new StringContent(json, Encoding.UTF8, "text/html"),
                });
            var client = new HttpClient(mockHttpMessageHandler.Object)
            {
                BaseAddress = new Uri(someOptions.Value.Urls.BaseUrl)
            };
            _mockFactory
                .Setup(_ => _.CreateClient("stack-gamer"))
                .Returns(client);

            _loggerMock = new Mock<ILogger<global::Fetcher.Thief>>();

            _thief = new global::Fetcher.Thief(_mockFactory.Object, someOptions, _loggerMock.Object);
        }

        [Test]
        public async Task Fetcher_Thief_ProductNullIfJsonError()
        {

            //https://www.thecodebuzz.com/unit-test-mock-httpclientfactory-moq-net-core/
            //https://stackoverflow.com/questions/56064031/mocking-httpclient-getasync-by-using-moq-library-in-xunit-test
            string json = "{\"coins\":0,\"coins_forzada:null,\"coins_sale\":null,\"destacado\":0,\"nombre\":\"Parlantes Edifier X100 2.1 \",\"id_categoria\":5,\"id_subcategoria\":65,\"garantia\":\"6 meses\",\"id_marca\":130,\"precioEspecial\":5409,\"precioLista\":5778,\"precioListaAnterior\":null,\"precioEspecialAnterior\":null,\"imagenes\":null,\"vendible\":0,\"codigo\":\"SIN STOCK\",\"niveles\":null,\"reviews\":null,\"tv\":null,\"sale\":null,\"incluidos\":null}";
            var status = HttpStatusCode.OK;

            CreateMocks(out Mock<ILogger<global::Fetcher.Thief>> _loggerMock, out global::Fetcher.Thief _thief, out Mock<IHttpClientFactory> _mockFactory, json, status);

            var productId = 1000;
            var result = await _thief.GetProductById(productId);

            _loggerMock.Verify(
                x => x.Log(
                    It.Is<LogLevel>(l => l == LogLevel.Error),
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => true),
                    It.IsAny<Exception>(),
                    It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)));

            //Assert
            Assert.IsNull(result);
        }

        [Test]
        public async Task Fetcher_Thief_ProductNullIfHttpStatusCodeNot200()
        {

            //https://www.thecodebuzz.com/unit-test-mock-httpclientfactory-moq-net-core/
            //https://stackoverflow.com/questions/56064031/mocking-httpclient-getasync-by-using-moq-library-in-xunit-test
            string json = "{\"coins\":0,\"coins_forzada\":null,\"coins_sale\":null,\"destacado\":0,\"nombre\":\"Parlantes Edifier X100 2.1 \",\"id_categoria\":5,\"id_subcategoria\":65,\"garantia\":\"6 meses\",\"id_marca\":130,\"precioEspecial\":5409,\"precioLista\":5778,\"precioListaAnterior\":null,\"precioEspecialAnterior\":null,\"imagenes\":null,\"vendible\":0,\"codigo\":\"SIN STOCK\",\"niveles\":null,\"reviews\":null,\"tv\":null,\"sale\":null,\"incluidos\":null}";
            var status = HttpStatusCode.NotFound;

            CreateMocks(out Mock<ILogger<global::Fetcher.Thief>> _loggerMock, out global::Fetcher.Thief _thief, out Mock<IHttpClientFactory> _mockFactory, json, status);

            var productId = 1000;
            var result = await _thief.GetProductById(productId);

            _loggerMock.Verify(
                x => x.Log(
                    It.Is<LogLevel>(l => l == LogLevel.Error),
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => true),
                    It.IsAny<Exception>(),
                    It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)));

            //Assert
            Assert.IsNull(result);
        }



    }
}
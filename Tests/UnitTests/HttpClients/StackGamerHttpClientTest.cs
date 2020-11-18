using Microsoft.Extensions.Options;
using NUnit.Framework;
using Shared.Options;
using System;
using System.Net;
using System.Threading.Tasks;

namespace Tests.UnitTests.HttpClients
{
    [TestFixture]
    public class StackGamerHttpClientTest
    {
        [Test]
        public async Task HttpClients_StackGamer_Should_Return_Http_Status_Code_And_Json_String()
        {
            // Setup
            var someOptions = Options.Create(new StackGamerOption()
            {
                Urls = new StackGamerOption.UrlsOption()
                {
                    BaseUrl = "https://test.com",
                    GetProductByIdUrl = "/test/get_product_by_id_url?id_producto="
                }
            });
            string json = "{\"coins\":0,\"coins_forzada\":null,\"coins_sale\":null,\"destacado\":0,\"nombre\":\"Parlantes Edifier X100 2.1 \",\"id_categoria\":5,\"id_subcategoria\":65,\"garantia\":\"6 meses\",\"id_marca\":130,\"precioEspecial\":5409,\"precioLista\":5778,\"precioListaAnterior\":null,\"precioEspecialAnterior\":null,\"imagenes\":null,\"vendible\":0,\"codigo\":\"SIN STOCK\",\"niveles\":null,\"reviews\":null,\"tv\":null,\"sale\":null,\"incluidos\":null}";
            var mockHttpClient = Mocks.MocksCreator.CreateHttpClientFactory(HttpStatusCode.OK,
                                                                        json,
                                                                        someOptions.Value.Urls.BaseUrl,
                                                                        Shared.Common.Constants.HTTP_CLIENT_STACK_GAMER);
            //Test
            var httpClient = mockHttpClient.Object.CreateClient(Shared.Common.Constants.HTTP_CLIENT_STACK_GAMER);

            Assert.IsNotNull(httpClient);

            Assert.AreEqual(new Uri(someOptions.Value.Urls.BaseUrl).ToString(), httpClient.BaseAddress.ToString());

            var response = await httpClient.GetAsync(string.Empty);

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

            var jsonRes = await response.Content.ReadAsStringAsync();

            Assert.AreEqual(json, jsonRes);
        }
    }
}

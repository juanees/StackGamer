using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Tests.Mocks
{
    public static class MocksCreator
    {
        public static Mock<ILogger<T>> CreateLoggerMock<T>()
        {
            return new Mock<ILogger<T>>();
        }

        public static Mock<IHttpClientFactory> CreateHttpClientFactory(HttpStatusCode returnHttpStatus,
                                                                       string returnJson,
                                                                       string baseAddress,
                                                                       string clientName,
                                                                       string returnMediaType = "text/html")
        {
            var _mockFactory = new Mock<IHttpClientFactory>();
            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = returnHttpStatus,
                    Content = new StringContent(returnJson, Encoding.UTF8, returnMediaType),
                });
            var client = new HttpClient(mockHttpMessageHandler.Object)
            {
                BaseAddress = new Uri(baseAddress)
            };
            _mockFactory
                .Setup(_ => _.CreateClient(clientName))
                .Returns(client);

            return _mockFactory;
        }
    }
}

using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
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
                                                                       string returnMediaType = "text/html",
                                                                       string acceptedMediaType= "text/html",
                                                                       int timeOutMili = 30000)
        {
            var _mockFactory = new Mock<IHttpClientFactory>();
            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            var ct = new CancellationTokenSource(timeOutMili).Token;

            CancellationTokenSource cts = new CancellationTokenSource();
            CancellationToken token = cts.Token;
            cts.CancelAfter(timeOutMili);
            token.ThrowIfCancellationRequested();


            mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = returnHttpStatus,
                    Content = new StringContent(returnJson, Encoding.UTF8, returnMediaType)
                });
            var client = new HttpClient(mockHttpMessageHandler.Object)
            {
                BaseAddress = new Uri(baseAddress)
            };
            //client.Timeout = TimeSpan.FromMilliseconds(timeOutMili);
            client.DefaultRequestHeaders.Accept.Clear();            
            _mockFactory
                .Setup(_ => _.CreateClient(clientName))
                .Returns(client);

            return _mockFactory;
        }
    }
}

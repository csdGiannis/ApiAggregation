using System.Net;
using ApiAggregation.Infrastructure.Metrics;
using NSubstitute;

namespace ApiAggregation.Infrastructure.Tests.Metrics
{
    public class MetricsHttpClientHandlerTests
    {
        private readonly IMetricsService _metricsService;
        private readonly TestHttpMessageHandler _innerHandler;
        private readonly HttpClient _httpClient;
        private const string ClientName = "TestClient";

        public MetricsHttpClientHandlerTests()
        {
            _metricsService = Substitute.For<IMetricsService>();
            _innerHandler = new TestHttpMessageHandler(); // Custom handler to mock behavior
            var handler = new MetricsHttpClientHandler(_metricsService, ClientName)
            {
                InnerHandler = _innerHandler
            };
            _httpClient = new HttpClient(handler);
        }

        [Fact]
        public async Task SendAsync_ShouldIncrementClientFast_WhenElapsedTimeIsBelow100ms()
        {
            // Arrange
            _innerHandler.Response = new HttpResponseMessage(HttpStatusCode.OK);
            _innerHandler.SimulatedDelayMs = 50;

            // Act
            var request = new HttpRequestMessage(HttpMethod.Get, "http://localhost/test");
            await _httpClient.SendAsync(request);

            // Assert
            _metricsService.Received(1).IncrementClientFast(ClientName);
        }

        [Fact]
        public async Task SendAsync_ShouldIncrementClientNormal_WhenElapsedTimeIsBetween100msAnd200ms()
        {
            // Arrange
            _innerHandler.Response = new HttpResponseMessage(HttpStatusCode.OK);
            _innerHandler.SimulatedDelayMs = 150;

            // Act
            var request = new HttpRequestMessage(HttpMethod.Get, "http://localhost/test");
            await _httpClient.SendAsync(request);

            // Assert
            _metricsService.Received(1).IncrementClientNormal(ClientName);
        }

        [Fact]
        public async Task SendAsync_ShouldIncrementClientSlow_WhenElapsedTimeIsAbove200ms()
        {
            // Arrange
            _innerHandler.Response = new HttpResponseMessage(HttpStatusCode.OK);
            _innerHandler.SimulatedDelayMs = 250; 

            // Act
            var request = new HttpRequestMessage(HttpMethod.Get, "http://localhost/test");
            await _httpClient.SendAsync(request);

            // Assert
            _metricsService.Received(1).IncrementClientSlow(ClientName);
        }

        [Fact]
        public async Task SendAsync_ShouldIncrementClientFailed_WhenResponseIsNotSuccessful()
        {
            // Arrange
            _innerHandler.Response = new HttpResponseMessage(HttpStatusCode.InternalServerError); 

            // Act
            var request = new HttpRequestMessage(HttpMethod.Get, "http://localhost/test");
            await _httpClient.SendAsync(request);

            // Assert
            _metricsService.Received(1).IncrementClientFailed(ClientName);
        }

        [Fact]
        public async Task SendAsync_ShouldIncrementClientFailed_WhenExceptionIsThrown()
        {
            // Arrange
            _innerHandler.ThrowException = true; 

            // Act & Assert
            var request = new HttpRequestMessage(HttpMethod.Get, "http://localhost/test");
            await Assert.ThrowsAsync<HttpRequestException>(() => _httpClient.SendAsync(request));

            _metricsService.Received(1).IncrementClientFailed(ClientName);
        }

    
        private class TestHttpMessageHandler : HttpMessageHandler
        {
            public HttpResponseMessage Response { get; set; } = new HttpResponseMessage(HttpStatusCode.OK);
            public int SimulatedDelayMs { get; set; }
            public bool ThrowException { get; set; }

            protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                if (ThrowException)
                {
                    throw new HttpRequestException("Simulated exception");
                }


                await Task.Delay(SimulatedDelayMs, cancellationToken);

                return Response;
            }
        }
    }
}
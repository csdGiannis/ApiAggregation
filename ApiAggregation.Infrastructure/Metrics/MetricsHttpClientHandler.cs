using System.Diagnostics;

namespace ApiAggregation.Infrastructure.Metrics
{

    //this is a DelegatingHandler that will be used to measure the time taken by the HttpClient to make a request
    //it is an interceptor that will be called before and after the HttpClient makes a request
    internal class MetricsHttpClientHandler : DelegatingHandler
    {
        private readonly IMetricsService _metricsService;
        private readonly string _clientName;

        public MetricsHttpClientHandler(IMetricsService metricsService, string clientName)
        {
            _metricsService = metricsService;
            _clientName = clientName;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var stopwatch = Stopwatch.StartNew();

            try
            {
                var response = await base.SendAsync(request, cancellationToken);
                stopwatch.Stop();

                var elapsedTime = stopwatch.ElapsedMilliseconds;
                if (response.IsSuccessStatusCode == false)
                {
                    _metricsService.IncrementClientFailed(_clientName);
                }
                else if (elapsedTime > 200)
                {
                    _metricsService.IncrementClientSlow(_clientName);
                }
                else if (elapsedTime > 100)
                {
                    _metricsService.IncrementClientNormal(_clientName);
                }
                else
                {
                    _metricsService.IncrementClientFast(_clientName);
                }

                return response;
            }
            catch (Exception)
            {
                _metricsService.IncrementClientFailed(_clientName);
                throw;
            }
        }
    }
}

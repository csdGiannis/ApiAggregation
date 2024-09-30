using System.Collections.Concurrent;

namespace ApiMetricsDemo.Metrics
{
    public class MetricsService : IMetricsService
    {

        private readonly ConcurrentDictionary<string, RequestMetrics> _clientMetrics = new ConcurrentDictionary<string, RequestMetrics>();


        public void IncrementClientSlow(string clientName) => _clientMetrics.GetOrAdd(clientName, new RequestMetrics()).IncrementSlow();
        public void IncrementClientNormal(string clientName) => _clientMetrics.GetOrAdd(clientName, new RequestMetrics()).IncrementNormal();
        public void IncrementClientFast(string clientName) => _clientMetrics.GetOrAdd(clientName, new RequestMetrics()).IncrementFast();
        public void IncrementClientFailed(string clientName) => _clientMetrics.GetOrAdd(clientName, new RequestMetrics()).IncrementFailed();

        public RequestMetrics GetClientMetrics(string clientName) => _clientMetrics.GetOrAdd(clientName, new RequestMetrics());

        public ConcurrentDictionary<string, RequestMetrics> GetAllClientMetrics() => _clientMetrics;


        public bool ResetClientMetrics(string? clientName = null)
        {
            if (clientName != null)
            {
               return _clientMetrics.TryRemove(clientName, out _);
            }
            _clientMetrics.Clear();
            return true;
        }
    }
}

using System.Collections.Concurrent;

namespace ApiAggregation.Infrastructure.Metrics
{
    public interface IMetricsService
    {
        public ConcurrentDictionary<string, RequestMetrics> GetAllClientMetrics();
        public RequestMetrics GetClientMetrics(string clientName);
        public void IncrementClientFailed(string clientName);
        public void IncrementClientFast(string clientName);
        public void IncrementClientNormal(string clientName);
        public void IncrementClientSlow(string clientName);
        public bool ResetClientMetrics(string? clientName = null);
    }
}
using System.Collections.Concurrent;

namespace ApiMetricsDemo.Metrics
{
    public interface IMetricsService
    {
        ConcurrentDictionary<string, RequestMetrics> GetAllClientMetrics();
        RequestMetrics GetClientMetrics(string clientName);
        void IncrementClientFailed(string clientName);
        void IncrementClientFast(string clientName);
        void IncrementClientNormal(string clientName);
        void IncrementClientSlow(string clientName);
        bool ResetClientMetrics(string? clientName = null);
    }
}
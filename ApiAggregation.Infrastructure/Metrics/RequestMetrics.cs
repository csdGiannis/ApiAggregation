namespace ApiMetricsDemo.Metrics
{
    public class RequestMetrics
    {
        private long _slowRequests;
        private long _normalRequests;
        private long _fastRequests;
        private long _failedRequests;

        public void IncrementSlow() => Interlocked.Increment(ref _slowRequests);
        public void IncrementNormal() => Interlocked.Increment(ref _normalRequests);
        public void IncrementFast() => Interlocked.Increment(ref _fastRequests);
        public void IncrementFailed() => Interlocked.Increment(ref _failedRequests);

        public long SlowRequests => Interlocked.Read(ref _slowRequests);
        public long NormalRequests => Interlocked.Read(ref _normalRequests);
        public long FastRequests => Interlocked.Read(ref _fastRequests);
        public long FailedRequests => Interlocked.Read(ref _failedRequests);
    }
}

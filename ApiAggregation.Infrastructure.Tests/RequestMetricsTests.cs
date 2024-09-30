using ApiAggregation.Infrastructure.Metrics;

namespace ApiAggregation.Infrastructure.Tests.Metrics
{
    public class RequestMetricsTests
    {
        [Fact]
        public void IncrementSlow_IncreasesSlowRequestCount()
        {
            // Arrange
            var metrics = new RequestMetrics();

            // Act
            metrics.IncrementSlow();

            // Assert
            Assert.Equal(1, metrics.SlowRequests);
        }

        [Fact]
        public void IncrementNormal_IncreasesNormalRequestCount()
        {
            // Arrange
            var metrics = new RequestMetrics();

            // Act
            metrics.IncrementNormal();

            // Assert
            Assert.Equal(1, metrics.NormalRequests);
        }

        [Fact]
        public void IncrementFast_IncreasesFastRequestCount()
        {
            // Arrange
            var metrics = new RequestMetrics();

            // Act
            metrics.IncrementFast();

            // Assert
            Assert.Equal(1, metrics.FastRequests);
        }

        [Fact]
        public void IncrementFailed_IncreasesFailedRequestCount()
        {
            // Arrange
            var metrics = new RequestMetrics();

            // Act
            metrics.IncrementFailed();

            // Assert
            Assert.Equal(1, metrics.FailedRequests);
        }

        [Fact]
        public void MultipleIncrements_IncreaseCorrectCounts()
        {
            // Arrange
            var metrics = new RequestMetrics();

            // Act
            metrics.IncrementSlow();
            metrics.IncrementSlow();
            metrics.IncrementNormal();
            metrics.IncrementFast();
            metrics.IncrementFailed();
            metrics.IncrementFailed();

            // Assert
            Assert.Equal(2, metrics.SlowRequests);
            Assert.Equal(1, metrics.NormalRequests);
            Assert.Equal(1, metrics.FastRequests);
            Assert.Equal(2, metrics.FailedRequests);
        }
    }
}

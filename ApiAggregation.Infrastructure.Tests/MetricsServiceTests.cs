using System.Collections.Concurrent;
using ApiAggregation.Infrastructure.Metrics;

namespace ApiAggregation.Infrastructure.Tests.Metrics;

public class MetricsServiceTests
{
    private readonly MetricsService _metricsService;

    public MetricsServiceTests()
    {
        _metricsService = new MetricsService();
    }

    [Fact]
    public void IncrementClientSlow_ShouldIncrementSlowRequests()
    {
        // Arrange
        var clientName = "TestClient";

        // Act
        _metricsService.IncrementClientSlow(clientName);

        // Assert
        var metrics = _metricsService.GetClientMetrics(clientName);
        Assert.Equal(1, metrics.SlowRequests);
        Assert.Equal(0, metrics.NormalRequests);
        Assert.Equal(0, metrics.FastRequests);
        Assert.Equal(0, metrics.FailedRequests);
    }

    [Fact]
    public void IncrementClientNormal_ShouldIncrementNormalRequests()
    {
        // Arrange
        var clientName = "TestClient";

        // Act
        _metricsService.IncrementClientNormal(clientName);

        // Assert
        var metrics = _metricsService.GetClientMetrics(clientName);
        Assert.Equal(1, metrics.NormalRequests);
        Assert.Equal(0, metrics.SlowRequests);
        Assert.Equal(0, metrics.FastRequests);
        Assert.Equal(0, metrics.FailedRequests);
    }

    [Fact]
    public void IncrementClientFast_ShouldIncrementFastRequests()
    {
        // Arrange
        var clientName = "TestClient";

        // Act
        _metricsService.IncrementClientFast(clientName);

        // Assert
        var metrics = _metricsService.GetClientMetrics(clientName);
        Assert.Equal(1, metrics.FastRequests);
        Assert.Equal(0, metrics.NormalRequests);
        Assert.Equal(0, metrics.SlowRequests);
        Assert.Equal(0, metrics.FailedRequests);
    }

    [Fact]
    public void IncrementClientFailed_ShouldIncrementFailedRequests()
    {
        // Arrange
        var clientName = "TestClient";

        // Act
        _metricsService.IncrementClientFailed(clientName);

        // Assert
        var metrics = _metricsService.GetClientMetrics(clientName);
        Assert.Equal(1, metrics.FailedRequests);
        Assert.Equal(0, metrics.NormalRequests);
        Assert.Equal(0, metrics.SlowRequests);
        Assert.Equal(0, metrics.FastRequests);
    }

    [Fact]
    public void GetAllClientMetrics_ShouldReturnAllMetrics()
    {
        // Arrange
        var client1 = "Client1";
        var client2 = "Client2";
        
        _metricsService.IncrementClientSlow(client1);
        _metricsService.IncrementClientNormal(client2);

        // Act
        ConcurrentDictionary<string, RequestMetrics> allMetrics = _metricsService.GetAllClientMetrics();

        // Assert
        Assert.True(allMetrics.ContainsKey(client1));
        Assert.True(allMetrics.ContainsKey(client2));
        Assert.Equal(1, allMetrics[client1].SlowRequests);
        Assert.Equal(1, allMetrics[client2].NormalRequests);
    }

    [Fact]
    public void ResetClientMetrics_ShouldClearSpecificClientMetrics()
    {
        // Arrange
        var clientName = "TestClient";
        _metricsService.IncrementClientSlow(clientName);

        // Act
        var result = _metricsService.ResetClientMetrics(clientName);

        // Assert
        Assert.True(result);
        Assert.False(_metricsService.GetAllClientMetrics().ContainsKey(clientName));
    }

    [Fact]
    public void ResetClientMetrics_ShouldClearAllMetrics_WhenClientNameIsNull()
    {
        // Arrange
        _metricsService.IncrementClientSlow("Client1");
        _metricsService.IncrementClientNormal("Client2");

        // Act
        var result = _metricsService.ResetClientMetrics();

        // Assert
        Assert.False(result);
        Assert.Empty(_metricsService.GetAllClientMetrics());
    }
}
using Microsoft.AspNetCore.Mvc;


namespace ApiMetricsDemo.Metrics;

[ApiController]
[Route("api/[controller]")]
public class MetricsController : ControllerBase
{
    private readonly IMetricsService _metricsService;

    public MetricsController(IMetricsService metricsService)
    {
        _metricsService = metricsService;
    }

    /// <summary>
    /// Resets all client metrics.
    /// </summary>
    /// <returns>A response indicating the metrics have been reset.</returns>
    [HttpPost("reset")]
    //[SwaggerOperation(Summary = "Resets all client metrics.")]
    //[SwaggerResponse(200, "Metrics have been reset.")]
    public IActionResult Reset()
    {
        _metricsService.ResetClientMetrics();
        return Ok(new { message = "Metrics have been reset." });
    }

    /// <summary>
    /// Resets metrics for a specific client.
    /// </summary>
    /// <param name="clientName">The name of the client.</param>
    /// <returns>A response indicating the client's metrics have been reset.</returns>
    [HttpPost("reset-client")]
    //[SwaggerOperation(Summary = "Resets metrics for a specific client.")]
    //[SwaggerResponse(200, "Client metrics have been reset.")]
    public IActionResult Reset(string clientName)
    {
        var success = _metricsService.ResetClientMetrics(clientName);
        if (!success)
        {

        }
        return Ok(new { message = $"{clientName} metrics have been reset." });
    }

    /// <summary>
    /// Gets metrics for all clients.
    /// </summary>
    /// <returns>A response containing metrics for all clients.</returns>
    [HttpGet("clients")]
    //[SwaggerOperation(Summary = "Gets metrics for all clients.")]
    //[SwaggerResponse(200, "Metrics for all clients.")]
    public IActionResult GetAllClientMetrics()
    {
        var allClientMetrics = _metricsService.GetAllClientMetrics()
            .ToDictionary(
                kvp => kvp.Key,
                kvp => new
                {
                    SlowRequests = kvp.Value.SlowRequests,
                    NormalRequests = kvp.Value.NormalRequests,
                    FastRequests = kvp.Value.FastRequests,
                    FailedRequests = kvp.Value.FailedRequests
                });

        return Ok(allClientMetrics);
    }

    /// <summary>
    /// Gets metrics for a specific client.
    /// </summary>
    /// <param name="clientName">The name of the client.</param>
    /// <returns>A response containing the client's metrics.</returns>
    [HttpGet("clients/{clientName}")]
    //[SwaggerOperation(Summary = "Gets metrics for a specific client.")]
    //[SwaggerResponse(200, "Metrics for the specified client.")]
    //[SwaggerResponse(404, "Metrics for the specified client not found.")]
    public IActionResult GetClientMetrics(string clientName)
    {
        var clientMetrics = _metricsService.GetClientMetrics(clientName);

        var metrics = new
        {
            SlowRequests = clientMetrics.SlowRequests,
            NormalRequests = clientMetrics.NormalRequests,
            FastRequests = clientMetrics.FastRequests,
            FailedRequests = clientMetrics.FailedRequests
        };

        return Ok(metrics);
    }
}

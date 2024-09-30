using ApiAggregation.Infrastructure.Metrics;
using Microsoft.AspNetCore.Mvc;

namespace ApiAggregation.API.Controllers
{
    [ApiController]
    [Route("api/metrics")]
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
        public IActionResult Reset()
        {
            _metricsService.ResetClientMetrics();
            return Ok(new { message = "Metrics have been reset." });
        }

        /// <summary>
        /// Gets metrics for all clients.
        /// </summary>
        /// <returns>A response containing metrics for all clients.</returns>
        [HttpGet("clients")]
        public IActionResult GetAllClientMetrics()
        {
            var allClientMetrics = _metricsService.GetAllClientMetrics()
                .ToDictionary(
                    kvp => kvp.Key,
                    kvp => new
                    {
                        kvp.Value.SlowRequests,
                        kvp.Value.NormalRequests,
                        kvp.Value.FastRequests,
                        kvp.Value.FailedRequests
                    });

            return Ok(allClientMetrics);
        }
    }
}

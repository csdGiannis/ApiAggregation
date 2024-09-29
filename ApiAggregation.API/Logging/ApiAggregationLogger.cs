using ApiAggregation.API.Middleware;
using ApiAggregation.SharedUtilites;

namespace ApiAggregation.API.Logging
{
    public class ApiAggregationLogger : IApiAggregationLogger
    {
        private readonly ILogger<ErrorHandlingMiddleware> _logger;

        public ApiAggregationLogger(ILogger<ErrorHandlingMiddleware> logger)
        {
            _logger = logger;
        }

        public void LogError(string message)
        {
            _logger.LogError(message);
        }
    }
}

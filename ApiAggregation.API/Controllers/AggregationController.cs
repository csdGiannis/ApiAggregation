using ApiAggregation.Application.DTOs;
using ApiAggregation.Application.Errors;
using ApiAggregation.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace ApiAggregation.API.Controllers
{
    [ApiController]
    [Route("api/aggregation")]
    public sealed class AggregationController : ControllerBase
    {
        private readonly IDataAggregationService _dataAggregationService;

        public AggregationController(IDataAggregationService dataAggregationService)
        {
            _dataAggregationService = dataAggregationService ?? throw new ArgumentNullException(nameof(dataAggregationService));
        }

        /// <summary>Fetches news articles from the News API based on the provided keyword.  </summary>
        /// <remarks>
        /// Parameters are optional filters coming from the user.
        /// Errorhandling mostly is taken care of from the Errorhandling middlware which we introduce after UseRouting() at the Program.cs.
        /// For custom error messaged a RestException object is used, along with the desired httpstatus code and a custom message.
        /// Format accepted is JSON and output is JSON and XML, all controlled in the Program.cs file, along with the custom logger,
        /// for both console and daily file logging (minimum level set is Warning).
        /// </remarks>
        /// <returns>An IEnumerable of type AggregatedDataDto object, merging all external API data</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AggregratedDataDto>>> GetAggregatedData([FromQuery] List<string>? countryNames = null)
        {
            var aggregatedData = await _dataAggregationService.GetAggregatedDataAsync(countryNames);

            if (aggregatedData == null)
                throw new RestException(HttpStatusCode.BadRequest, $"Unable to retrieve {nameof(aggregatedData)}");

            return Ok(aggregatedData);
        }

    }
}

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

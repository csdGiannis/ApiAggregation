using ApiAggregation.Application.DTOs;
using ApiAggregation.Application.Errors;
using ApiAggregation.Application.Interfaces;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace ApiAggregation.API.Controllers
{
    public class AggregationControllerValidator : AbstractValidator<RequestQuery>
    {
        public AggregationControllerValidator()
        {
            RuleFor(r => r.CountryNames)
                .Must(countryNames => countryNames.Count <= 10).WithMessage("The request accepts up to 10 countries at a time.")
                .ForEach(countryName =>
                {
                    countryName.MaximumLength(50).WithMessage("Country's character limit is 50.");
                });
            RuleFor(r => r.KeyWords)
                .Must(keyWords => keyWords.Count <= 10).WithMessage("The request accepts up to 10 keywords at a time.")
                .ForEach(keyWord =>
                {
                    keyWord.MaximumLength(20).WithMessage("Keyword's character limit is 20.");
                });
        }
    }

    [ApiController]
    [Route("api/aggregation")]
    public class AggregationController : ControllerBase
    {
        private readonly IDataAggregationService _dataAggregationService;

        public AggregationController(IDataAggregationService dataAggregationService)
        {
            _dataAggregationService = dataAggregationService ?? throw new ArgumentNullException(nameof(dataAggregationService));
        }

        /// <summary>Fetches news articles from the News API based on the provided keyword.  </summary>
        /// <remarks>
        /// 1)Errorhandling mostly is taken care of from the Errorhandling middlware which we introduce after UseRouting() at the Program.cs.
        /// For custom error messaged a RestException object is used, along with the desired httpstatus code and a custom message.
        /// Also Fluent Validation is added for validating the request parameters if any.
        /// 2)Format accepted is JSON and output is JSON and XML, all configured in the Program.cs file
        /// 3)Custom logger Serilog is initialized,for both console and daily file logging (minimum level set is Warning for the file logs).
        /// </remarks>
        /// <param name="requestParameters">RequestQuery requestParameters are optional query parameters for filtering and sorting</param>
        /// <param name="cancellationToken">Cancellation token is used so the request can cancel if the user decides to cancel/refresh the action</param>
        /// <returns>An IEnumerable of type AggregatedDataDto object, merging all external API data</returns>
        [HttpGet]
        public async Task<ActionResult<AggregratedDataDto>> GetAggregatedData([FromQuery] RequestQuery requestParameters,
                                                                                           CancellationToken cancellationToken)
        {
            var aggregatedData = await _dataAggregationService.GetAggregatedDataAsync(requestParameters, cancellationToken);

            if (aggregatedData == null)
                throw new RestException(HttpStatusCode.BadRequest, $"Unable to retrieve {nameof(aggregatedData)}");

            return Ok(aggregatedData);
        }

    }
}

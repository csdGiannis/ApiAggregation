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
            RuleFor(r => r.PublishYear)
                .Must(publishYear => publishYear != null ?
                    publishYear <= DateTime.Now.Year && publishYear >= 1700 : true)
                    .WithMessage("The publish year can be from 1700 to current year.");
            RuleFor(r => r.PageSize)
                    .Must(pageSize => pageSize <= 100 && pageSize >= 10).WithMessage("The page size can be set from 10 to 100");
            RuleFor(r => r.PageNumber)
                    .Must(pageNumber => pageNumber <= 200 && pageNumber >= 1).WithMessage("The page number can be set from 1 to 200");
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

        /// <summary>
        /// Retrieves aggregated data from external APIs based on the specified filter parameters.
        /// </summary>
        /// <remarks>
        /// 1) Fluent Validation is added for validating the request parameters.
        /// 2) Accepted input format: JSON via query parameters. The API supports both JSON and XML as output formats based on the client's Accept header.
        /// 3) A custom logger is used to log requests and responses to both the console and a file for debugging (minimum level for the logging file is set to Warning).
        /// </remarks>
        /// <param name="requestParameters">The filter parameters used for aggregating data. These are optional query parameters except of page number and size.</param>
        /// <param name="cancellationToken">Token to monitor for cancellation requests, allowing the operation to be cancelled if necessary.</param>
        /// <returns>
        /// An <see cref="ActionResult"/> containing the aggregated data based on the applied filters.
        /// </returns>
        /// <response code="200">Returns the aggregated data.</response>
        /// <response code="400">Bad request.</response>
        /// <response code="406">Not acceptable. Supported output formats are JSON and XML.</response>
        /// <response code="500">Server error.</response>
        [HttpGet]
        [ProducesResponseType(200)]
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

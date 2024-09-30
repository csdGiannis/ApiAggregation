using ApiAggregation.Application.DTOs;
using ApiAggregation.Application.Errors;
using ApiAggregation.Application.Interfaces;
using ApiAggregation.Application.Interfaces.ExternalData;
using ApiAggregation.Application.Mappers;
using ApiAggregation.Domain.DomainModels;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Polly.CircuitBreaker;
using System.Net;

namespace ApiAggregation.Application.Services
{
    public class DataAggregationService : IDataAggregationService
    {
        private readonly ICountriesDataProvider _countriesDataProvider;
        private readonly INewsDataProvider _newsDataProvider;
        private readonly ILibraryDataProvider _libraryDataProvider;
        private readonly ILogger<DataAggregationService> _logger;
        private readonly IMemoryCache _memoryCache;


        public DataAggregationService(ICountriesDataProvider countriesDataProvider, INewsDataProvider newsDataProvider,
                                        ILibraryDataProvider libraryDataProvider, ILogger<DataAggregationService> logger,
                                        IMemoryCache memoryCache)
        {
            _countriesDataProvider = countriesDataProvider ?? throw new ArgumentNullException(nameof(countriesDataProvider));
            _newsDataProvider = newsDataProvider ?? throw new ArgumentNullException(nameof(newsDataProvider));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _libraryDataProvider = libraryDataProvider ?? throw new ArgumentNullException(nameof(libraryDataProvider));
            _memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
        }

        /// <summary>Service responsible for implementing the simultaneous API calls after fetching and mapping the data from the clients.</summary>
        /// <remarks>
        /// First Domain models are created from the external data providers asynchronously.
        /// Then the Domain models are mapped into the corresponding DTOs and group on the final aggregated Dtos to return to the controller
        /// </remarks>
        /// <returns>An IEnumerable AggregatedDataDto object which contains all the external APIs data grouped per country</returns>
        public async Task<AggregratedDataDto> GetAggregatedDataAsync(RequestQuery requestParameters, CancellationToken cancellationToken)
        {
            //Validating the inputted countries from the user. An Immutable Array is locally used. In real world situation a frequently updated database would be used.
            //It removes every bad user input.
            requestParameters.CountryNames = UtilityService.ValidateCountries(requestParameters.CountryNames);

            //If the collection returned after the validation is empty, throws an exception
            if (!requestParameters.CountryNames.Any())
                throw new RestException(HttpStatusCode.NotFound, "No countries found. Please check your inputs.");

            //Adjusting the List of keywords inputted by the User.
            requestParameters.KeyWords = requestParameters.KeyWords.Any() ? UtilityService.FormatStringInputList(requestParameters.KeyWords)
                                                                : new List<string>();

            //Simultaneously starting the tasks while using Circuit breaker pattern to trigger when any external API keeps returning bad requests.
            var countriesTask = ExecuteCountriesTask(requestParameters, cancellationToken);
            var newsTask = ExecuteNewsTask(requestParameters, cancellationToken);
            var libraryTask = ExecuteLibraryTask(requestParameters, cancellationToken);

            //Completing tasks simultaneously
            await Task.WhenAll(countriesTask, newsTask, libraryTask);

            //Getting task results
            var countries = await countriesTask;
            var news = await newsTask;
            var library = await libraryTask;

            //Aggregate and return results if valid
            return MapAndAggregateDataPerCountry(countries, news, library, requestParameters);

        }

        /// <summary>Executes a task to return external API's data</summary>
        /// <remarks>
        /// Try-catch block provides logging if Circuit breaker triggers
        /// Allows 3 unsuccessful tries within 15 seconds separately. 
        /// Catching the error (before reaching the middleware error handler) allows to log it, while the application is still running 
        /// </remarks>
        /// <returns>An IEnumerable of the corresponding Domain model</returns>
        private async Task<IEnumerable<Country>> ExecuteCountriesTask(RequestQuery requestParameters, CancellationToken cancellationToken)
        {
            try
            {
                //Caching in memory if key(country name) is not found, otherwise gets country data from memory
                List<Country> cachedCountries = new List<Country>();
                List<string> notCachedCountryNames = new List<string>();

                foreach (var countryString in requestParameters.CountryNames)
                {
                    if (_memoryCache.TryGetValue(countryString, out Country cachedCountry))
                    {
                        cachedCountries.Add(cachedCountry);
                    }
                    else
                    {
                        notCachedCountryNames.Add(countryString);
                    }
                }
                //saving the inputs temporarily until the request begins if needed
                var tempInputedCountryNames = requestParameters.CountryNames;
                requestParameters.CountryNames = notCachedCountryNames;

                //if there are countries that were not cached, continue with the external api request 
                if (notCachedCountryNames.Count > 0)
                {
                    var apiCountries = await _countriesDataProvider.GetCountries(requestParameters, cancellationToken);
                    cachedCountries.AddRange(apiCountries);

                    //Caches the new country results into memory
                    foreach (var apiCountry in apiCountries)
                    {
                        _memoryCache.Set(apiCountry.NameCommon.ToLower(), apiCountry, TimeSpan.FromMinutes(5));
                    }
                }
                //setting back the inputs
                requestParameters.CountryNames = tempInputedCountryNames;
                return cachedCountries;
            }
            catch (BrokenCircuitException bcEx)
            {
                _logger.LogError("Circuit breaker opened for RestCountries API. All RestCountries API's requests will be rejected until the circuit is closed.");
                return new List<Country>();
            }
        }

        /// <summary>Executes a task to return external API's data, similarly to ExecuteCountriesTask</summary>
        private async Task<News> ExecuteNewsTask(RequestQuery requestParameters, CancellationToken cancellationToken)
        {
            try
            {
                return await _newsDataProvider.GetNews(requestParameters, cancellationToken);
            }
            catch (BrokenCircuitException bcEx)
            {
                _logger.LogError("Circuit breaker opened for News API. All News API's requests will be rejected until the circuit is closed.");
                return new News();
            }
        }


        /// <summary>Executes a task to return external API's data, similarly to ExecuteCountriesTask</summary>
        private async Task<Library> ExecuteLibraryTask(RequestQuery requestParameters, CancellationToken cancellationToken)
        {
            try
            {
                return await _libraryDataProvider.GetLibrary(requestParameters, cancellationToken);
            }
            catch (BrokenCircuitException bcEx)
            {
                _logger.LogError("Circuit breaker opened for OpenLibrary API. All News API's requests will be rejected until the circuit is closed.");
                return new Library();
            }
        }

        /// <summary>Private method responsible mapping the Domain models into Dtos</summary>
        /// <param name="countries">The mapped domain model returned from the RestCountries API</param>
        /// <param name="news">The mapped domain model returned from the News API</param>
        /// <param name="library">The mapped domain model returned from the OpenLibrary API</param>
        /// <param name="requestParameters">The optional filtering options</param>
        /// <returns>An filtered AggregatedDataDto object which contains all the external APIs data</returns>
        private AggregratedDataDto MapAndAggregateDataPerCountry(IEnumerable<Country> countries, News news, Library library,
                                                                RequestQuery requestParameters)
        {
            try
            {
                //merging the results into one list of RelevantPrints objects 
                AggregratedDataDto aggregratedData = new AggregratedDataDto
                {
                    CountriesInformation = countries.ToCountriesDto().OrderByDescending(x => x.Population),
                    RelevantPrints = RelevantPrintDtoMapper.ToRelevantPrints(library, news),
                    PageNumber = requestParameters.PageNumber,
                    PageSize = requestParameters.PageSize
                };

                //apply filters and  pagination
                aggregratedData.RelevantPrints = aggregratedData.RelevantPrints
                    //filtering
                    .Where(b => requestParameters.IsBook == true ? b.IsBook == true : requestParameters.IsBook == false ? b.IsBook == false : true)
                    .Where(p => requestParameters.PublishYear != null ? p.PublishYear == requestParameters.PublishYear.ToString() : true)
                    //sorting
                    .OrderBy(t => requestParameters.SortOrder.ToLower() == "ascending" && requestParameters.SortField.ToLower() == "title" ? t.Title : null)
                    .OrderBy(t => requestParameters.SortOrder.ToLower() == "ascending" && requestParameters.SortField.ToLower() == "publishyear" ? int.TryParse(t.PublishYear, out int result) ? result : int.MinValue : int.MinValue)
                    .OrderBy(t => requestParameters.SortOrder.ToLower() == "ascending" && requestParameters.SortField.ToLower() == "source" ? t.Source : null)
                    .OrderBy(t => requestParameters.SortOrder.ToLower() == "ascending" && requestParameters.SortField.ToLower() == "description" ? t.Description : null)
                    .OrderBy(t => requestParameters.SortOrder.ToLower() == "ascending" && requestParameters.SortField.ToLower() == "url" ? t.Url : null)
                    .OrderBy(t => requestParameters.SortOrder.ToLower() == "descending" && requestParameters.SortField.ToLower() == "isbook" ? t.IsBook : false)//booleans have to sort opposite way

                    .OrderByDescending(t => requestParameters.SortOrder.ToLower() == "descending" && requestParameters.SortField.ToLower() == "title" ? t.Title : null)
                    .OrderByDescending(t => requestParameters.SortOrder.ToLower() == "descending" && requestParameters.SortField.ToLower() == "publishyear" ? int.TryParse(t.PublishYear, out int result) ? result : int.MinValue : int.MinValue)
                    .OrderByDescending(t => requestParameters.SortOrder.ToLower() == "descending" && requestParameters.SortField.ToLower() == "source" ? t.Source : null)
                    .OrderByDescending(t => requestParameters.SortOrder.ToLower() == "descending" && requestParameters.SortField.ToLower() == "description" ? t.Description : null)
                    .OrderByDescending(t => requestParameters.SortOrder.ToLower() == "descending" && requestParameters.SortField.ToLower() == "url" ? t.Url : null)
                    .OrderByDescending(t => requestParameters.SortOrder.ToLower() == "ascending" && requestParameters.SortField.ToLower() == "isbook" ? t.IsBook : false) //booleans have to sort opposite way
                    .ToList();

                //total number of prints after filtering
                aggregratedData.TotalPrintResults = aggregratedData.RelevantPrints.Count;

                //apply pagination
                aggregratedData.RelevantPrints = aggregratedData.RelevantPrints
                                                    .Skip((requestParameters.PageNumber - 1) * requestParameters.PageSize)
                                                    .Take(requestParameters.PageSize)
                                                    .ToList();

                //number of prints on current selected page
                aggregratedData.PrintsOnCurrentPage = aggregratedData.RelevantPrints.Count;

                return aggregratedData;
            }
            catch
            {
                throw new RestException(HttpStatusCode.BadRequest, "Error while filtering. Please check your filter inputs.");
            }
        }
    }
}

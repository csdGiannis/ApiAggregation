using ApiAggregation.Application.DTOs;
using ApiAggregation.Application.Errors;
using ApiAggregation.Application.Interfaces;
using ApiAggregation.Application.Interfaces.ExternalData;
using ApiAggregation.Application.Mappers;
using ApiAggregation.Domain.DomainModels;
using ApiAggregation.SharedUtilites;
using Polly.CircuitBreaker;
using System.Net;

namespace AggregatedApi.Application.Services;

public class DataAggregationService : IDataAggregationService
{
    private readonly ICountriesDataProvider _countriesDataProvider;
    private readonly INewsDataProvider _newsDataProvider;
    private readonly ILibraryDataProvider _libraryDataProvider;
    private readonly IApiAggregationLogger _logger;

    public DataAggregationService(ICountriesDataProvider countriesDataProvider, INewsDataProvider newsDataProvider,
                                    ILibraryDataProvider libraryDataProvider, IApiAggregationLogger logger)
    {
        _countriesDataProvider = countriesDataProvider ?? throw new ArgumentNullException(nameof(countriesDataProvider));
        _newsDataProvider = newsDataProvider ?? throw new ArgumentNullException(nameof(newsDataProvider));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _libraryDataProvider = libraryDataProvider ?? throw new ArgumentNullException(nameof(libraryDataProvider));
    }

    /// <summary>Service responsible for implementing the simultaneous API calls after fetching and mapping the data from the clients.</summary>
    /// <remarks>
    /// First Domain models are created from the external data providers asynchronously.
    /// Then the Domain models are mapped into the corresponding DTOs and group on the final aggregated Dtos to return to the controller
    /// </remarks>
    /// <returns>An IEnumerable AggregatedDataDto object which contains all the external APIs data grouped per country</returns>
    public async Task<AggregratedDataDto> GetAggregatedDataAsync(RequestQuery requestParameters, CancellationToken cancellationToken)
    {
        //Adjusting the List of strings inputted by the User. Defaults: Country Names - "Greece", KeyWords - empty
        requestParameters.CountryNames= FormatStringInputList(requestParameters.CountryNames);
        requestParameters.KeyWords = requestParameters.KeyWords.Any() ? FormatStringInputList(requestParameters.KeyWords)
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

        //if the user has not inputed any valid country names, no results should be returned
        if (countries.Any())
            return MapAndAggregateDataPerCountry(countries, news, library, requestParameters);
        else
            throw new RestException(HttpStatusCode.BadRequest, "Error: No valid country names are given.");
    }

    /// <summary>Executes a task to return external API's data</summary>
    /// <remarks>
    /// Try-catch block provides logging if Circuit breaker triggers
    /// Allows 3 unsuccessful tries within 20 seconds separately. 
    /// Catching the error (before reaching the middleware error handler) allows to log it, while the application is still running 
    /// </remarks>
    /// <returns>An IEnumerable of the corresponding Domain model</returns>
    private async Task<IEnumerable<Country>> ExecuteCountriesTask(RequestQuery requestParameters, CancellationToken cancellationToken)
    {
        try
        {
            return await _countriesDataProvider.GetCountries(requestParameters, cancellationToken);
        }
        catch (BrokenCircuitException bcEx)
        {
            _logger.LogError("Circuit breaker opened for RestCountries API. All RestCountries API's requests will be rejected until the circuit is closed.");
            return new List<Country>();
        }
    }

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
        //merging the results into one list of RelevantPrints objects 
        AggregratedDataDto aggregratedData = new AggregratedDataDto
        {
            CountriesInformation = countries.ToCountriesDto(),
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
            .OrderBy(t => requestParameters.SortOrder.ToLower() == "ascending" && requestParameters.SortField.ToLower() == "publishyear" ? (int.TryParse(t.PublishYear, out int result) ? result : int.MinValue) : int.MinValue)
            .OrderBy(t => requestParameters.SortOrder.ToLower() == "ascending" && requestParameters.SortField.ToLower() == "source" ? t.Source : null)
            .OrderBy(t => requestParameters.SortOrder.ToLower() == "ascending" && requestParameters.SortField.ToLower() == "description" ? t.Description : null)
            .OrderBy(t => requestParameters.SortOrder.ToLower() == "ascending" && requestParameters.SortField.ToLower() == "url" ? t.Url : null)
            .OrderBy(t => requestParameters.SortOrder.ToLower() == "descending" && requestParameters.SortField.ToLower() == "isbook" ? t.IsBook : false)//booleans have to sort opposite way

            .OrderByDescending(t => requestParameters.SortOrder.ToLower() == "descending" && requestParameters.SortField.ToLower() == "title" ? t.Title : null)
            .OrderByDescending(t => requestParameters.SortOrder.ToLower() == "descending" && requestParameters.SortField.ToLower() == "publishyear" ? (int.TryParse(t.PublishYear, out int result) ? result : int.MinValue) : int.MinValue)
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

    /// <summary>Formats the IEnumerable of strings to trimmed,lower,non-empty and not-null List of strings</summary>
    private ICollection<string> FormatStringInputList(IEnumerable<string> inputList) =>
        inputList.Where(x => !string.IsNullOrEmpty(x))
                 .ToList()
                 .ConvertAll(x => x.Trim().ToLower());

}

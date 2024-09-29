using ApiAggregation.Application.DTOs;
using ApiAggregation.Application.Interfaces;
using ApiAggregation.Application.Interfaces.ExternalData;
using ApiAggregation.Application.Mappers;
using ApiAggregation.Domain.DomainModels;
using ApiAggregation.SharedUtilites;
using Polly.CircuitBreaker;

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
        var countryNames = FormatStringInputList(requestParameters.CountryNames);
        var keyWords = requestParameters.KeyWords.Count > 0 ? FormatStringInputList(requestParameters.KeyWords)
                                                            : new List<string>();

        ////The news/book keywords depend on the country the user has inputted in order to get relevant results
        ////The country names are added to the list
        //keyWords.AddRange(countryNames);

        //Simultaneously starting the tasks while using Circuit breaker pattern to trigger when any external API keeps returning bad requests.
        var countriesTask = ExecuteCountriesTask(countryNames, cancellationToken);
        var newsTask = ExecuteNewsTask(countryNames: countryNames, keyWords: keyWords, cancellationToken);
        var libraryTask = ExecuteLibraryTask(countryNames: countryNames, keyWords: keyWords, cancellationToken);

        //Completing tasks simultaneously
        await Task.WhenAll(countriesTask, newsTask, libraryTask);

        //Getting task results
        var countries = await countriesTask;
        var news = await newsTask;
        var library = await libraryTask;

        //if the user has not inputed any valid country names, no results should be returned
        if (countries.Any())
            return MapAndAggregateDataPerCountry(countries, news, library);

        return new AggregratedDataDto();
    }

    /// <summary>Executes a task to return external API's data</summary>
    /// <remarks>
    /// Try-catch block provides logging if Circuit breaker triggers
    /// Allows 3 unsuccessful tries within 20 seconds separately. 
    /// Catching the error (before reaching the middleware error handler) allows to log it, while the application is still running 
    /// </remarks>
    /// <returns>An IEnumerable of the corresponding Domain model</returns>
    private async Task<IEnumerable<Country>> ExecuteCountriesTask(IEnumerable<string> keyWords, CancellationToken cancellationToken)
    {
        try
        {
            return await _countriesDataProvider.GetCountries(keyWords, cancellationToken);
        }
        catch (BrokenCircuitException bcEx)
        {
            _logger.LogError("Circuit breaker opened for RestCountries API. All RestCountries API's requests will be rejected until the circuit is closed.");
            return new List<Country>();
        }
    }

    private async Task<News> ExecuteNewsTask(IEnumerable<string> countryNames, IEnumerable<string> keyWords,
                                             CancellationToken cancellationToken)
    {
        try
        {
            return await _newsDataProvider.GetNews(countryNames: countryNames, keyWords: keyWords, cancellationToken);
        }
        catch (BrokenCircuitException bcEx)
        {
            _logger.LogError("Circuit breaker opened for News API. All News API's requests will be rejected until the circuit is closed.");
            return new News();
        }
    }

    private async Task<Library> ExecuteLibraryTask(IEnumerable<string> countryNames, IEnumerable<string> keyWords,
                                                    CancellationToken cancellationToken)
    {
        try
        {
            return await _libraryDataProvider.GetLibrary(countryNames: countryNames, keyWords: keyWords, cancellationToken);
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
    /// <returns>An AggregatedDataDto object which contains all the external APIs data</returns>
    private AggregratedDataDto MapAndAggregateDataPerCountry(IEnumerable<Country> countries,
                                                             News news,
                                                             Library library)
    {       
        AggregratedDataDto aggregratedData = new AggregratedDataDto
        {
            CountriesInformation = countries.ToCountriesDto(),
            News = news.ToNewsDto(),
            Library = library.ToLibraryDto()
        };

        return aggregratedData;
    }

    /// <summary>Formats the IEnumerable of strings to trimmed,lower,non-empty and not-null List of strings</summary>
    private IEnumerable<string> FormatStringInputList(IEnumerable<string> inputList) =>
        inputList.Where(x => !string.IsNullOrEmpty(x))
                 .ToList()
                 .ConvertAll(x => x.Trim().ToLower());

}

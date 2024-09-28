using ApiAggregation.Application.DTOs;
using ApiAggregation.Application.Errors;
using ApiAggregation.Application.Interfaces;
using ApiAggregation.Application.Interfaces.ExternalData;
using ApiAggregation.Application.Mappers;
using System.Net;

namespace AggregatedApi.Application.Services;

public class DataAggregationService : IDataAggregationService
{
    private readonly ICountriesDataProvider _countriesDataProvider;
    private readonly INewsDataProvider _newsDataProvider;

    public DataAggregationService(ICountriesDataProvider countriesDataProvider, INewsDataProvider newsDataProvider)
    {
        _countriesDataProvider = countriesDataProvider ?? throw new ArgumentNullException(nameof(countriesDataProvider));
        _newsDataProvider = newsDataProvider ?? throw new ArgumentNullException(nameof(newsDataProvider));
    }

    /// <summary>Service responsible for implementing the simultaneous API calls after fetching and mapping the data from the clients.</summary>
    /// <remarks>
    /// First Domain models are created from the external data providers asynchronously.
    /// Then the Domain models are mapped into the corresponding DTOs and group on the final aggregated Dtos to return to the controller
    /// </remarks>
    /// <returns>An IEnumerable AggregatedDataDto object which contains all the external APIs data grouped per country</returns>
    public async Task<IEnumerable<AggregratedDataDto>> GetAggregatedDataAsync(RequestQuery requestParameters, CancellationToken cancellationToken)
    {
        var countryNames = requestParameters.CountryNames.ConvertAll(x => x.Trim().ToLower()); //Trim for imput fix and toLower to ignore casing at searching

        //simultaneously starting the tasks
        var countryInformationTask = _countriesDataProvider.GetCountryInformation(countryNames, cancellationToken);
        var newsInformationTask = _newsDataProvider.GetNewsInformation(countryNames, cancellationToken);
        //third api

        await Task.WhenAll(countryInformationTask, newsInformationTask); //takes care of waiting till all simultaneous tasks end

        var countryInformation = await countryInformationTask;
        var newsInformation = await newsInformationTask;

        if (countryInformation == null)
            throw new RestException(HttpStatusCode.BadRequest, $"Error retrieving {nameof(countryInformation)}");

        var aggregatedData = new List<AggregratedDataDto>();
        foreach (var country in countryNames)
        {
            //map to DTO per country and article list and add to the AggegatedDataDto
            var mappedCountry = countryInformation.Where(x => x.NameCommon.ToLower() == country
                                                        || x.NameOfficial.ToLower().Contains(country)
                                                    ).FirstOrDefault()?.ToCountryInformationDto();


            var mappedNews = newsInformation.Where(x => x.Country == country).FirstOrDefault()?.ToNewsInformationDto(); //extention method for mapping to Dto 

            if (mappedCountry != null)
            {
                aggregatedData.Add(new AggregratedDataDto
                {
                    CountryName = country,
                    CountryInformation = mappedCountry,
                    NewsInformation = mappedNews ?? new List<ArticleDto>()
                });
            }    
        }

        return aggregatedData;
    }
}

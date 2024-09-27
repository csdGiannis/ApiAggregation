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

    public async Task<AggregratedDataDto> GetAggregatedDataAsync(List<string>? countryNames)
    {
        //Default country list contains Greece
        countryNames ??= new List<string> { "Greece" };

        countryNames = countryNames.ConvertAll(x => x.ToLower()); //toLower to ignore casing at searching

        var countryInformation = await _countriesDataProvider.GetCountryInformation(countryNames);
        var newsInformation = await _newsDataProvider.GetNewsInformation(countryNames);
        //third api

        //.whenall use

        if (countryInformation == null)
            throw new RestException(HttpStatusCode.BadRequest, $"Error retrieving {nameof(countryInformation)}");

        var countryData = new List<CountryInformationDto>();

        foreach (var country in countryInformation)
        {
            var mappedCountry = country?.ToCountryInformationDto();
            if (mappedCountry != null)
                countryData.Add(mappedCountry);
        }

        var aggregatedData = new AggregratedDataDto()
        {
            CountryInformation = countryData
        };

        return aggregatedData;
    }
}

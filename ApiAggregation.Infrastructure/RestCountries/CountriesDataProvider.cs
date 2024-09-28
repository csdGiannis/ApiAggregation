using ApiAggregation.Application.DTOs;
using ApiAggregation.Application.Interfaces.ExternalData;
using ApiAggregation.Domain.DomainModels;
using ApiAggregation.Infrastructure.RestCountries.ResponseObjects;
using Newtonsoft.Json;
using System.Diagnostics.Metrics;

namespace ApiAggregation.Infrastructure.RestCountries;

public class CountriesDataProvider : ICountriesDataProvider
{
    private readonly HttpClient _httpClient;

    public CountriesDataProvider(HttpClient httpClient)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
    }

    /// <summary>
    /// The data provider responsible for returning the RestCountries API data after mapping it to the Domain Model CountryInformation
    /// </summary>
    public async Task<IEnumerable<CountryInformation>> GetCountryInformation(List<string> countryNames, CancellationToken cancellationToken)
    {
        var countryInformationFromExternalApi = await GetCountryInformationFromExternalApi(countryNames, cancellationToken);

        var countryData = new List<CountryInformation>();

        foreach (var externalCountry in countryInformationFromExternalApi)
        {
            //extention method for mapping response object into data model
            var mappedCountry = externalCountry?.ToCountryInformation();
            if (mappedCountry != null)
                countryData.Add(mappedCountry);
        }

        return countryData;
    }

    /// <summary>
    /// This method is responsible for fetching the country data from the RestCountries API,deserializing it with the help of Newtonsoft.Json to a response object
    /// </summary>
    private async Task<IEnumerable<RestCountriesResponse>> GetCountryInformationFromExternalApi(List<string> countryNames, CancellationToken cancellationToken)
    {
        var response = await _httpClient.GetAsync($"https://restcountries.com/v3.1/all", cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            return new List<RestCountriesResponse>();
        }

        var responseAsString = await response.Content.ReadAsStringAsync();

        var countries = JsonConvert.DeserializeObject<List<RestCountriesResponse>>(responseAsString);

        if (countries == null || countries.Count == 0)
        {
            return new List<RestCountriesResponse>();
        }

        return countries.Where(x => countryNames.Contains(x.Name.Official.ToLower()) || countryNames.Contains(x.Name.Common.ToLower()));
    }
}

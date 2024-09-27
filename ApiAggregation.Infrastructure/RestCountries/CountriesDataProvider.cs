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
    public async Task<IEnumerable<CountryInformation>> GetCountryInformation(List<string> countryNames)
    {
        var countryInformationFromExternalApi = await GetCountryInformationFromExternalApi(countryNames);

        var countryData = new List<CountryInformation>();

        foreach (var externalCountry in countryInformationFromExternalApi)
        {
            var mappedCountry = externalCountry?.ToCountryInformation();
            if (mappedCountry != null)
                countryData.Add(mappedCountry);
        }

        return countryData;
    }

    public async Task<IEnumerable<string>> GetEuropeanCountryNames()
    {
        var countryInformationFromExternalApi = await GetEuropeanCountryNamesFromExternalApi();

        var countryData = new List<string>();

        foreach (var externalCountry in countryInformationFromExternalApi)
        {
            var mappedCountry = externalCountry?.ToCountryInformation();
            if (mappedCountry != null)
                countryData.Add(mappedCountry.NameCommon);
        }

        return countryData;

    }

    private async Task<IEnumerable<RestCountriesResponse>> GetCountryInformationFromExternalApi(List<string> countryNames)
    {
        var response = await _httpClient.GetAsync($"https://restcountries.com/v3.1/all");
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

    private async Task<IEnumerable<RestCountriesResponse>> GetEuropeanCountryNamesFromExternalApi()
    {
        var response = await _httpClient.GetAsync($"https://restcountries.com/v3.1/region/europe");
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

        return countries;
    }
}

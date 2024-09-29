using ApiAggregation.Domain.DomainModels;
using ApiAggregation.Infrastructure.RestCountries.ResponseObjects;

namespace ApiAggregation.Infrastructure.RestCountries;

public static class CountryMapper
{
    /// <summary>
    /// Maps RestCountries response object to the Domain model Country as an extention method
    /// </summary>
    /// <param name="countryResponse">The response object to be mapped to the Domain model</param>
    public static Country ToCountry(this RestCountriesResponse countryResponse)
    {
        return new Country
        {
            NameOfficial = countryResponse.Name.Official ?? string.Empty,
            NameCommon = countryResponse.Name.Common ?? string.Empty,
            Capital = countryResponse.Capital.FirstOrDefault() ?? string.Empty,
            Region = countryResponse.Region ?? string.Empty,
            Population = countryResponse.Population,
            Languages = countryResponse.Languages.Values.ToList()
        };
    }
}

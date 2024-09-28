using ApiAggregation.Domain.DomainModels;
using ApiAggregation.Infrastructure.RestCountries.ResponseObjects;

namespace ApiAggregation.Infrastructure.RestCountries;

public static class CountryInformationMapper
{
    /// <summary>
    /// Maps RestCountriesResponse response object to the Domain model CountryInformation as an extention method
    /// </summary>
    /// <param name="response">The response object to be mapped to the Domain model</param>
    public static CountryInformation ToCountryInformation(this RestCountriesResponse response)
    {
        return new CountryInformation
        {
            NameOfficial = response.Name.Official ?? string.Empty,
            NameCommon = response.Name.Common ?? string.Empty,
            Capital = response.Capital.FirstOrDefault() ?? string.Empty,
            Region = response.Region ?? string.Empty,
            Population = response.Population,
            Languages = response.Languages.Values.ToList()
        };
    }
}

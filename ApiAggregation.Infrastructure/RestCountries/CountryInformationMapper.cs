using ApiAggregation.Domain.DomainModels;
using ApiAggregation.Infrastructure.RestCountries.ResponseObjects;

namespace ApiAggregation.Infrastructure.RestCountries;

public static class CountryInformationMapper
{
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

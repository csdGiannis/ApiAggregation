using ApiAggregation.Application.DTOs;
using ApiAggregation.Domain.DomainModels;

namespace ApiAggregation.Application.Mappers
{
    /// <summary>
    /// Maps Country Domain models to the CountryDtos
    /// </summary>
    internal static class CountriesDtoMapper
    {
        internal static List<CountryDto> ToCountriesDto(this IEnumerable<Country> countries)
        {
            var mappedCountries = new List<CountryDto>();
            foreach (var country in countries)
            {
                mappedCountries.Add(new CountryDto
                {
                    NameCommon = country.NameCommon ?? string.Empty,
                    NameOfficial = country.NameOfficial ?? string.Empty,
                    Capital = country.Capital ?? string.Empty,
                    Region = country.Region ?? string.Empty,
                    Population = country.Population,
                    Languages = country.Languages ?? new List<string>()
                });
            }
            return mappedCountries;
        }
    }
}

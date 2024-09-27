using ApiAggregation.Application.DTOs;
using ApiAggregation.Domain.DomainModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiAggregation.Application.Mappers
{
    public static class CountryInformationDtoMapper
    {
        public static CountryInformationDto ToCountryInformationDto(this CountryInformation countryInformation)
        {
            return new CountryInformationDto
            {
                NameOfficial = countryInformation.NameOfficial ?? string.Empty,
                NameCommon = countryInformation.NameCommon ?? string.Empty,
                Capital = countryInformation.Capital ?? string.Empty,
                Region = countryInformation.Region ?? string.Empty,
                Population = countryInformation.Population,
                Languages = countryInformation.Languages
            };
        }
    }
}

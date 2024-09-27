using ApiAggregation.Domain.DomainModels;

namespace ApiAggregation.Application.Interfaces.ExternalData;

public interface ICountriesDataProvider
{
    Task<IEnumerable<CountryInformation>> GetCountryInformation(List<string> countryNames);
    Task<IEnumerable<string>> GetEuropeanCountryNames();
}

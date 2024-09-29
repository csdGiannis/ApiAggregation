using ApiAggregation.Domain.DomainModels;

namespace ApiAggregation.Application.Interfaces.ExternalData;

public interface ICountriesDataProvider
{
    Task<IEnumerable<Country>> GetCountries(IEnumerable<string> keyWords, CancellationToken cancellationToken);
}

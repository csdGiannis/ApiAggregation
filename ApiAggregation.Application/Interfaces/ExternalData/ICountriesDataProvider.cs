using ApiAggregation.Application.DTOs;
using ApiAggregation.Domain.DomainModels;

namespace ApiAggregation.Application.Interfaces.ExternalData
{
    public interface ICountriesDataProvider
    {
        Task<IEnumerable<Country>> GetCountries(RequestQuery requestParameters, CancellationToken cancellationToken);
    }
}

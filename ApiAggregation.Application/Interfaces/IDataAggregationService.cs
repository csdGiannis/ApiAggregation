using ApiAggregation.Application.DTOs;

namespace ApiAggregation.Application.Interfaces;

public interface IDataAggregationService
{
    Task<IEnumerable<AggregratedDataDto>> GetAggregatedDataAsync(RequestQuery requestParameters, CancellationToken cancellationToken);
}
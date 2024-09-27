using ApiAggregation.Application.DTOs;

namespace ApiAggregation.Application.Interfaces;

public interface IDataAggregationService
{
    Task<AggregratedDataDto> GetAggregatedDataAsync(List<string>? countryNames);
}
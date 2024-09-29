using ApiAggregation.Application.DTOs;
using ApiAggregation.Domain.DomainModels;

namespace ApiAggregation.Application.Interfaces.ExternalData
{
    public interface INewsDataProvider
    {
        Task<News> GetNews(RequestQuery requestParameters, CancellationToken cancellationToken);
    }
}

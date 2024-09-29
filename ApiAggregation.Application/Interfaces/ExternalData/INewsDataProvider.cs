using ApiAggregation.Domain.DomainModels;

namespace ApiAggregation.Application.Interfaces.ExternalData
{
    public interface INewsDataProvider
    {
        Task<News> GetNews(IEnumerable<string> countryNames, 
                            IEnumerable<string> keyWords,
                            CancellationToken cancellationToken);
    }
}

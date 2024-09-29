using ApiAggregation.Domain.DomainModels;

namespace ApiAggregation.Application.Interfaces.ExternalData
{
    public interface ILibraryDataProvider
    {
        Task<Library> GetLibrary(IEnumerable<string> countryNames, IEnumerable<string> keyWords, CancellationToken cancellationToken);
    }
}

using ApiAggregation.Application.DTOs;
using ApiAggregation.Domain.DomainModels;

namespace ApiAggregation.Application.Interfaces.ExternalData
{
    public interface ILibraryDataProvider
    {
        Task<Library> GetLibrary(RequestQuery requestParameters, CancellationToken cancellationToken);
    }
}

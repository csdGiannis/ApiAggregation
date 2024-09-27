using ApiAggregation.Domain.DomainModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiAggregation.Application.Interfaces.ExternalData
{
    public interface INewsDataProvider
    {
        Task<IEnumerable<NewsInformation>> GetNewsInformation(List<string> countryNames);
    }
}

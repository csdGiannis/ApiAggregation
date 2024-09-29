using ApiAggregation.Application.DTOs.Books;
using ApiAggregation.Application.DTOs.Country;
using ApiAggregation.Application.DTOs.News;

namespace ApiAggregation.Application.DTOs
{
    public class AggregratedDataDto
    {
        public IEnumerable<CountryDto> CountriesInformation { get; set; } = new List<CountryDto>();
        public NewsDto News { get; set; } = new NewsDto();
        public LibraryDto Library { get; set; } = new LibraryDto();
    }
}

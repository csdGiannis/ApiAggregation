namespace ApiAggregation.Application.DTOs
{
    public class AggregratedDataDto
    {
        public IEnumerable<CountryDto> CountriesInformation { get; set; } = new List<CountryDto>();
        public int TotalPrintResults { get; set; } = 0;
        public int PrintsOnCurrentPage { get; set; } = 0;
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public ICollection<RelevantPrintDto> RelevantPrints { get; set; } = new List<RelevantPrintDto>();
    }
}

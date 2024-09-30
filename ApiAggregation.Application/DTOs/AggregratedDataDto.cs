namespace ApiAggregation.Application.DTOs
{
    /// <summary>
    /// A DTO containing the aggregated data.
    /// </summary>
    public class AggregratedDataDto
    {
        /// <summary>
        /// A collection of CountryDtos that include basic countries information
        /// </summary>
        /// <remarks>Default contains the string "Greece".</remarks>
        public IEnumerable<CountryDto> CountriesInformation { get; set; } = new List<CountryDto>();

        /// <summary>
        /// A string containing the total number of prints found.
        /// </summary>
        public int TotalPrintResults { get; set; } = 0;

        /// <summary>
        /// An integer containing the number of prints returned based on the current chosen pagination.
        /// </summary>
        public int PrintsOnCurrentPage { get; set; } = 0;

        /// <summary>
        /// An integer containing the current page number for pagination.
        /// </summary>
        /// <remarks>Default is 1.</remarks>
        public int PageNumber { get; set; }

        /// <summary>
        /// An integer containing the current number of items per page for pagination.
        /// </summary>
        /// <remarks>Default is 10.</remarks>
        public int PageSize { get; set; }

        /// <summary>
        /// A collection of RelevantPrintDtos that include the returned prints.
        /// </summary>
        public ICollection<RelevantPrintDto> RelevantPrints { get; set; } = new List<RelevantPrintDto>();
    }
}

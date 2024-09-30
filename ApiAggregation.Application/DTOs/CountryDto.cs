namespace ApiAggregation.Application.DTOs
{
    /// <summary>
    /// A DTO containing each country's data.
    /// </summary>
    public class CountryDto
    {
        /// <summary>
        /// A string containing the country's common name. Usually short.
        /// </summary>
        public string NameCommon { get; set; } = string.Empty;

        /// <summary>
        /// A string containing the country's official name. Often longer.
        /// </summary>
        public string NameOfficial { get; set; } = string.Empty;

        /// <summary>
        /// A string containing the country's capital.
        /// </summary>
        public string Capital { get; set; } = string.Empty;

        /// <summary>
        /// An integer containing the country's population.
        /// </summary>
        public int Population { get; set; }

        /// <summary>
        /// A string containing the country's region.
        /// </summary>
        public string Region { get; set; } = string.Empty;

        /// <summary>
        /// A collection of strings containing the country's most spoken languages.
        /// </summary>
        public IEnumerable<string> Languages { get; set; } = new List<string>();
    }
}

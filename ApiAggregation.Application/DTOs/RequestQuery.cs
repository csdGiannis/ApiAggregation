using System.ComponentModel.DataAnnotations;

namespace ApiAggregation.Application.DTOs
{
    /// <summary>
    /// A DTO containing the query parameters for retrieving aggregated data.
    /// </summary>
    public class RequestQuery
    {
        /// <summary>
        /// An optional list of country names that are essential for filtering incoming results. The results must contain ALL the inputted countries.
        /// </summary>
        /// <remarks>Default contains the string "Greece".</remarks>
        public ICollection<string> CountryNames { get; set; } = new List<string> { "Greece" };

        /// <summary>
        /// An optional list of keywords that are optional for additional filtering incoming results. The results must contain ONE of they keywords.
        /// </summary>
        public ICollection<string> KeyWords { get; set; } = new List<string>();

        /// <summary>
        /// An optional filter parameter about whether the relative printing is a book if yes or an Article if false.
        /// </summary>
        public bool? IsBook { get; set; } = null;

        /// <summary>
        /// An optional filter parameter about the publish year of the printing (book or article).
        /// </summary>
        public int? PublishYear { get; set; }

        /// <summary>
        /// The page number for pagination.
        /// </summary>
        /// <remarks>Default is 1.</remarks>
        public int PageNumber { get; set; } = 1;

        /// <summary>
        /// The number of items per page for pagination.
        /// </summary>
        /// <remarks>Default is 10.</remarks>
        public int PageSize { get; set; } = 10;


        /// <summary>
        /// The sort order for the aggregated results (Input must be "ascending" or "descending" in order to work).
        /// </summary>
        public string SortOrder { get; set; } = string.Empty;

        /// <summary>
        /// The sort field for ordering the aggregated results. (Input must be one of the AggegatedDataDto's in order to work, for example "title").
        /// </summary>
        public string SortField { get; set; } = string.Empty;
    }
}

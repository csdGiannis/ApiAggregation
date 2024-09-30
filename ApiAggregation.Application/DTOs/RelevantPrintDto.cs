namespace ApiAggregation.Application.DTOs
{
    /// <summary>
    /// A DTO containing each print's data.
    /// </summary>
    public class RelevantPrintDto
    {
        /// <summary>
        /// A string containing the print's title.
        /// </summary>
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// A string containing the print's publish year.
        /// </summary>
        public string PublishYear { get; set; } = string.Empty;

        /// <summary>
        /// A string containing the print's description.
        /// </summary>
        /// <remarks>Only articles contain a description.</remarks>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// A string containing the print's source.
        /// </summary>
        /// <remarks>Only articles contain a source.</remarks>
        public string Source { get; set; } = string.Empty;

        /// <summary>
        /// A string containing the print's url for the full article.
        /// </summary>
        /// <remarks>Only articles contain a url.</remarks>
        public string Url { get; set; } = string.Empty;

        /// <summary>
        /// A list of strings containing the authors of the print.
        /// </summary>
        /// <remarks>Only books contain author names.</remarks>
        public List<string> AuthorName { get; set; } = new List<string>();

        /// <summary>
        /// A collection of strings containing the languages used on the print.
        /// </summary>
        /// <remarks>Only books contain information about the used languages.</remarks>
        public IEnumerable<string> Language { get; set; } = new List<string>();

        /// <summary>
        /// A boolean to state if the print is either a book or an article.
        /// </summary>
        public bool? IsBook { get; set; } = null;
    }
}

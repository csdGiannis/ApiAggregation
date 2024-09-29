namespace ApiAggregation.Application.DTOs.Books
{
    public class BookDto
    {
        public string Title { get; set; } = string.Empty;
        public List<string> AuthorName { get; set; } = new List<string>();
        public string PublishYear { get; set; } = string.Empty;
        public IEnumerable<string> Language { get; set; } = new List<string>();
    }
}

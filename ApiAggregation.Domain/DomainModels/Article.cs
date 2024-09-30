namespace ApiAggregation.Domain.DomainModels
{
    public class Article
    {
        public string Source { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public string PublishedAt { get; set; } = string.Empty;
    }
}

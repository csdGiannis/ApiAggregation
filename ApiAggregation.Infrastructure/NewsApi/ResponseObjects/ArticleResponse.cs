using Newtonsoft.Json;

namespace ApiAggregation.Infrastructure.NewsApi.ResponseObjects
{
    public class ArticleResponse
    {
        [JsonProperty("source")]
        public Source Source { get; set; } 

        [JsonProperty("author")]
        public string Author { get; set; } = string.Empty; 

        [JsonProperty("title")]
        public string Title { get; set; } = string.Empty; 

        [JsonProperty("description")]
        public string Description { get; set; } = string.Empty; 

        [JsonProperty("url")]
        public string Url { get; set; } = string.Empty; 

        [JsonProperty("urlToImage")]
        public string UrlToImage { get; set; } = string.Empty;

        [JsonProperty("publishedAt")]
        public DateTime PublishedAt { get; set; } 

        [JsonProperty("content")]
        public string Content { get; set; } = string.Empty;
    }
}

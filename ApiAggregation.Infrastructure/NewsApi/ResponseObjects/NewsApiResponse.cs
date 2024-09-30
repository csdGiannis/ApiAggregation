using Newtonsoft.Json;

namespace ApiAggregation.Infrastructure.NewsApi.ResponseObjects
{
    internal class NewsApiResponse
    {
        [JsonProperty("status")]
        public string Status { get; set; } = string.Empty;

        [JsonProperty("code")]
        public string Code { get; set; } = string.Empty;

        [JsonProperty("message")]
        public string Message { get; set; } = string.Empty;

        [JsonProperty("totalResults")]
        public int TotalResults { get; set; }

        [JsonProperty("articles")]
        public IEnumerable<ArticleResponse> Articles { get; set; } = new List<ArticleResponse>();
    }
}
    
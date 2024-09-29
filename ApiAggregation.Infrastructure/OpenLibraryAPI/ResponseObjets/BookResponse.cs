using Newtonsoft.Json;

namespace ApiAggregation.Infrastructure.OpenLibraryAPI.ResponseObjets
{
    public class BookResponse
    {
        [JsonProperty("title")]
        public string Title { get; set; } = string.Empty;

        [JsonProperty("author_name")]
        public List<string> AuthorName { get; set; } = new List<string>();

        [JsonProperty("publish_year")]
        public IEnumerable<int> PublishYear { get; set; } = new List<int>();

        [JsonProperty("language")]
        public IEnumerable<string> Language { get; set; } = new List<string>();
    }
}

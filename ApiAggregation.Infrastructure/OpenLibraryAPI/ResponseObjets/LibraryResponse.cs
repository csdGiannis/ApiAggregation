using Newtonsoft.Json;

namespace ApiAggregation.Infrastructure.OpenLibraryAPI.ResponseObjets
{
    public class LibraryResponse
    {
        [JsonProperty("docs")]
        public IEnumerable<BookResponse> Docs { get; set; } = new List<BookResponse>();

        [JsonProperty("start")]
        public int Start { get; set; }

        [JsonProperty("num_found")]
        public int NumFound { get; set; }
    }
}

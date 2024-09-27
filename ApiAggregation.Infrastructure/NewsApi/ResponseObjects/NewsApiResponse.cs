using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiAggregation.Infrastructure.NewsApi.ResponseObjects
{
    public class NewsApiResponse
    {
        [JsonProperty("status")]
        public string Status { get; set; } = string.Empty;

        [JsonProperty("totalResults")]
        public int TotalResults { get; set; }

        [JsonProperty("articles")]
        public IEnumerable<ArticleData> Articles { get; set; } = new List<ArticleData>();
    }
}
    
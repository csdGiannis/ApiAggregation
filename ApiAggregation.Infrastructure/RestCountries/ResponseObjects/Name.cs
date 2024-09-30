using Newtonsoft.Json;

namespace ApiAggregation.Infrastructure.RestCountries.ResponseObjects
{
    internal class Name
    {
        [JsonProperty("common")]
        public string Common { get; set; } = string.Empty;

        [JsonProperty("official")]
        public string Official { get; set; } = string.Empty;
    }
}

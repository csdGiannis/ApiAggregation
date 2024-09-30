using Newtonsoft.Json;

namespace ApiAggregation.Infrastructure.RestCountries.ResponseObjects;

internal class RestCountriesResponse
{
    [JsonProperty("name")]
    public Name Name { get; set; } = new Name();

    [JsonProperty("capital")]
    public List<string> Capital { get; set; } = new List<string>();

    [JsonProperty("region")]
    public string Region { get; set; } = string.Empty;

    [JsonProperty("subregion")]
    public string SubRegion { get; set; } = string.Empty;

    [JsonProperty("population")]
    public int Population { get; set; }

    [JsonProperty("languages")]
    public Dictionary<string, string> Languages { get; set; } = new Dictionary<string, string>();

    [JsonProperty("borders")]
    public List<string> Borders { get; set; } = new List<string>();
}



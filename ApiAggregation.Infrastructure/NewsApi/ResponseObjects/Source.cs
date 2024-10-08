﻿using Newtonsoft.Json;

namespace ApiAggregation.Infrastructure.NewsApi.ResponseObjects
{
    internal class Source
    {
        [JsonProperty("id")]
        public string Id { get; set; } = string.Empty;

        [JsonProperty("name")]
        public string Name { get; set; } = string.Empty;
    }
}

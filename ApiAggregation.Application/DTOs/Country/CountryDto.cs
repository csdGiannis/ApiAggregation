namespace ApiAggregation.Application.DTOs.Country
{
    public class CountryDto
    {
        public string NameCommon { get; set; } = string.Empty;
        public string NameOfficial { get; set; } = string.Empty;
        public string Capital { get; set; } = string.Empty;
        public int Population { get; set; }
        public string Region { get; set; } = string.Empty;
        public IEnumerable<string> Languages { get; set; } = new List<string>();
    }
}

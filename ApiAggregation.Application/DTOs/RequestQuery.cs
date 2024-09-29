namespace ApiAggregation.Application.DTOs
{
    public class RequestQuery
    {
        public List<string> CountryNames { get; set; } = new List<string> { "Greece" };
        public List<string> KeyWords { get; set; } = new List<string>();
    }
}

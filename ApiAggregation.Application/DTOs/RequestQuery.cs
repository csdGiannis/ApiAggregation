using System.ComponentModel.DataAnnotations;

namespace ApiAggregation.Application.DTOs
{
    public class RequestQuery
    {
        public ICollection<string> CountryNames { get; set; } = new List<string> { "Greece" };
        public ICollection<string> KeyWords { get; set; } = new List<string>();

        [Required]
        public int PageNumber { get; set; }

        [Required]
        public int PageSize { get; set; }
        public bool IsBook { get; set; } = false;
        public bool IsArticle { get; set; } = false;
        public int? PublishYear { get; set; }
        public string SortOrder { get; set; } = string.Empty;
        public string SortField { get; set; } = string.Empty;
    }
}

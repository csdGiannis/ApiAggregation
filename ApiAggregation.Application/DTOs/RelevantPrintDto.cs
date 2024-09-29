using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiAggregation.Application.DTOs
{
    public class RelevantPrintDto
    {
        public string Title { get; set; } = string.Empty;
        public string PublishYear { get; set; } = string.Empty;
        public string Source { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public List<string> AuthorName { get; set; } = new List<string>();

        public IEnumerable<string> Language { get; set; } = new List<string>();

        public bool IsBook { get; set; } = false;
        public bool IsArticle { get; set; } = false;
    }
}

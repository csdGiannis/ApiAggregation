using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiAggregation.Domain.DomainModels
{
    public class NewsInformation
    {
        public string Country {  get; set; } = string.Empty;
        public IEnumerable<Article> Articles { get; set; } = new List<Article>();
    }
}

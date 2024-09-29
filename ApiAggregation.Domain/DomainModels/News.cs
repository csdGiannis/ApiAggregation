using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiAggregation.Domain.DomainModels
{
    public class News
    {
        public int TotalResults { get; set; }
        public IEnumerable<Article> Articles { get; set; } = new List<Article>();
    }
}

using ApiAggregation.Domain.DomainModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiAggregation.Application.DTOs
{
    public class NewsInformationDto
    {
        public IEnumerable<Article> Articles { get; set; } = new List<Article>();
    }
}

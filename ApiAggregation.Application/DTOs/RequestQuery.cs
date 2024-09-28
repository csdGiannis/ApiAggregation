using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiAggregation.Application.DTOs
{
    public class RequestQuery
    {
        public List<string> CountryNames { get; set; } = new List<string> { "Greece" };
    }
}

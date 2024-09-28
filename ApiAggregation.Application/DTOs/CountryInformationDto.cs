using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiAggregation.Application.DTOs
{
    public class CountryInformationDto
    {
        public string NameOfficial { get; set; } = string.Empty;
        public string Capital { get; set; } = string.Empty;
        public int Population { get; set; }
        public string Region {  get; set; } = string.Empty;
        public List<string> Languages { get; set; } = new List<string>();
    }
}

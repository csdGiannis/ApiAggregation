using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiAggregation.Application.DTOs
{
    public class AggregratedDataDto
    {
        public IEnumerable<CountryInformationDto> CountryInformation { get; set; } = new List<CountryInformationDto>();
    }
}

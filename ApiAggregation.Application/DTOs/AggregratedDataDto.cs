using ApiAggregation.Domain.DomainModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiAggregation.Application.DTOs
{
    public class AggregratedDataDto
    {
        public string CountryName { get; set; } = string.Empty;
        public CountryInformationDto CountryInformation { get; set; } = new CountryInformationDto();
        public IEnumerable<ArticleDto> NewsInformation { get; set; } = new List<ArticleDto>();
    }
}

using ApiAggregation.Application.DTOs;
using ApiAggregation.Domain.DomainModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiAggregation.Application.Mappers
{
    /// <summary>
    /// Maps NewsInformation Domain model to the NewsInformationDto
    /// </summary>
    /// <param name="newsInformation">The Domain model to be mapped to the IEnumerable NewsInformationDto</param>
    public static class NewsInformationDtoMapper
    {
        public static IEnumerable<ArticleDto> ToNewsInformationDto(this NewsInformation newsInformation)
        {
            var mappedNewsInformationDto = new List<ArticleDto>();
            
            var articlesToBeMapped = newsInformation.Articles;
            foreach (var article in articlesToBeMapped)
            {
                mappedNewsInformationDto.Add(
                    new ArticleDto
                    {
                        PublishedAt = article.PublishedAt ?? "",
                        Source = article.Source ?? "",
                        Description = article.Description ?? "",
                        Title = article.Title ?? "",
                        Url = article.Url ?? "",
                    }
                );
            }
            return mappedNewsInformationDto;
        }
    }
}

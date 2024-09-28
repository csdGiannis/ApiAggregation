using ApiAggregation.Domain.DomainModels;
using ApiAggregation.Infrastructure.NewsApi.ResponseObjects;
using ApiAggregation.Infrastructure.RestCountries.ResponseObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiAggregation.Infrastructure.NewsApi
{
    public static class NewsInformationMapper
    {
        /// <summary>
        /// Maps ArticleData response object to the Domain model Article as an extention method
        /// </summary>
        /// <param name="articlesToMap">The response object to be mapped to the Domain model</param>
        public static IEnumerable<Article> ToArticles(this List<ArticleData> articlesToMap)
        {
            var mappedArticles = new List<Article>();
            foreach (var article in articlesToMap)
            {
                mappedArticles.Add(
                    new Article
                    {
                        Source = article.Source != null ? article.Source.Name : string.Empty,
                        Title = article.Title ?? string.Empty,
                        Description = article.Description ?? string.Empty,
                        Url = article.Url ?? string.Empty,
                        PublishedAt = article.PublishedAt.ToShortDateString()
                    }
                );
            }
            return mappedArticles;
        }
    }
}

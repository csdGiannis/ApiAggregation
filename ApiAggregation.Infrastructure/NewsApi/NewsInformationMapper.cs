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
        public static IEnumerable<Article> ToArticles(this List<ArticleData> articlesToMap)
        {
            var mappedArticles = new List<Article>();
            foreach (var article in articlesToMap)
            {
                mappedArticles.Add(
                    new Article
                    {
                        Source = article.Source.Name,
                        Title = article.Title,
                        Content = article.Content,
                        Description = article.Description,
                        Url = article.Url,
                        PublishedAt = article.PublishedAt.ToShortDateString()
                    }
                );
            }
            return mappedArticles;
        }
    }
}

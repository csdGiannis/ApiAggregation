using ApiAggregation.Domain.DomainModels;
using ApiAggregation.Infrastructure.NewsApi.ResponseObjects;

namespace ApiAggregation.Infrastructure.NewsApi
{
    public static class NewsMapper
    {
        /// <summary>
        /// Maps News response objects to the News Domain model as an extention method
        /// </summary>
        public static News ToNews(this NewsApiResponse newsResponse)
        {
            List<Article> mappedArticles = new();

            var articleResponse = newsResponse.Articles;
            foreach (var article in articleResponse)
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

            return new News
            {
                TotalResults = newsResponse.TotalResults,
                Articles = mappedArticles
            };
        }
    }
}

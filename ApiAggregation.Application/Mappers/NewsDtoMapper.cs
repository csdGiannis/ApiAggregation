using ApiAggregation.Application.DTOs.News;
using ApiAggregation.Domain.DomainModels;

namespace ApiAggregation.Application.Mappers
{
    /// <summary>
    /// Maps News Domain model to the NewsDto
    /// </summary>
    public static class NewsDtoMapper
    {
        public static NewsDto ToNewsDto(this News news)
        {
            List<ArticleDto> mappedArticlesDto = new();
            
            var articlesToBeMapped = news.Articles;
            foreach (var article in articlesToBeMapped)
            {
                mappedArticlesDto.Add(
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


            return new NewsDto
            {
                TotalResults = news.TotalResults,
                ArticlesDto = mappedArticlesDto
            };
        }
    }
}

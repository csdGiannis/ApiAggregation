using ApiAggregation.Application.DTOs;
using ApiAggregation.Domain.DomainModels;

namespace ApiAggregation.Application.Mappers
{
    /// <summary>
    /// Maps Library and News Domain models to the RelevantPrintDtos
    /// </summary>
    public static class RelevantPrintDtoMapper
    {
        public static ICollection<RelevantPrintDto> ToRelevantPrints(Library library, News news)
        {
            List<RelevantPrintDto> mappedRelevantPrintDtos = new();

            //mapping library-books
            var books = library.Books;
            foreach (var book in books)
            {
                mappedRelevantPrintDtos.Add(
                    new RelevantPrintDto
                    {
                        Title = book.Title ?? string.Empty,
                        AuthorName = book.AuthorName,
                        PublishYear = book.PublishYear ?? string.Empty,
                        Language = book.Language,
                        IsBook = true
                    }
                );
            }
            //mapping news-articles
            var articlesToBeMapped = news.Articles;
            foreach (var article in articlesToBeMapped)
            {
                mappedRelevantPrintDtos.Add(
                    new RelevantPrintDto
                    {
                        PublishYear = article.PublishedAt ?? "",
                        Source = article.Source ?? "",
                        Description = article.Description ?? "",
                        Title = article.Title ?? "",
                        Url = article.Url ?? "",
                        IsArticle = true
                    }
                );
            }

            return mappedRelevantPrintDtos;
        }
    }
}

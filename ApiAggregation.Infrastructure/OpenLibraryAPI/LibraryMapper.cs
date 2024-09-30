using ApiAggregation.Domain.DomainModels;
using ApiAggregation.Infrastructure.OpenLibraryAPI.ResponseObjets;

namespace ApiAggregation.Infrastructure.OpenLibraryAPI
{
    public static class LibraryMapper
    {
        /// <summary>
        /// Maps Library response objects to the Library Domain models as an extention method
        /// </summary>
        public static Library ToLibrary(this LibraryResponse libraryResponse)
        {
            List<Book> mappedBooks = new();

            var bookResponse = libraryResponse.Docs;
            foreach (var book in bookResponse)
            {
                mappedBooks.Add(
                    new Book
                    {
                        AuthorName = book.AuthorName ?? new List<string>(),
                        Title = book.Title ?? string.Empty,
                        Language = book.Language ?? new List<string>(),
                        PublishYear = book.PublishYear.FirstOrDefault() > 0 ? book.PublishYear.FirstOrDefault().ToString() : ""
                    }
                );
            }
            return new Library
            {
                TotalResults = libraryResponse.NumFound,
                Books = mappedBooks
            };
        }
    }
}

using ApiAggregation.Application.DTOs.Books;
using ApiAggregation.Domain.DomainModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiAggregation.Application.Mappers
{
    /// <summary>
    /// Maps Library Domain model to the Library Dto
    /// </summary>
    public static class LibraryDtoMapper
    {
        public static LibraryDto ToLibraryDto(this Library library)
        {
            List<BookDto> mappedBooksDto = new();

            var books = library.Books;
            foreach (var book in books)
            {
                mappedBooksDto.Add(
                    new BookDto
                    {
                        Title = book.Title ?? string.Empty,
                        AuthorName = book.AuthorName,
                        PublishYear = book.PublishYear,
                        Language = book.Language
                    }
                );
            }
            return new LibraryDto
            {
                TotalResults = library.TotalResults,
                BooksDto = mappedBooksDto
            };
        }
    }
}

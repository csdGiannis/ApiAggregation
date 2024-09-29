namespace ApiAggregation.Application.DTOs.Books
{
    public class LibraryDto
    {
        public int TotalResults { get; set; }
        public List<BookDto> BooksDto { get; set; } = new List<BookDto>();
    }
}

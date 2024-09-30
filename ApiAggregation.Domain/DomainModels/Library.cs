namespace ApiAggregation.Domain.DomainModels
{
    public class Library
    {
        public int TotalResults { get; set; }
        public List<Book> Books { get; set; } = new List<Book>();
    }
}

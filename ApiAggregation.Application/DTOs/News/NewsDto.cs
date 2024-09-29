namespace ApiAggregation.Application.DTOs.News
{
    public class NewsDto
    {
        public int TotalResults { get; set; }
        public IEnumerable<ArticleDto> ArticlesDto { get; set; } = new List<ArticleDto>();
    }
}

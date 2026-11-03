namespace News_Portal_API.DTOs.News
{
    public class ArticleDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string ShortDescription { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public int ReadTimeMinutes { get; set; }
        public bool IsTrending { get; set; }

        public CategoryDto? Category { get; set; }
        public AuthorDto? Author { get; set; }
        public ArticleStatsDto? Stats { get; set; }
    }
}

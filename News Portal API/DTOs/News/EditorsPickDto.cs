namespace News_Portal_API.DTOs.News
{

    public class EditorsPickDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;

        public EditorsPickCategoryDto? Category { get; set; }
        public ArticleStatsDto? Stats { get; set; }
    }

    public class EditorsPickCategoryDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? BadgeColor { get; set; }
    }
}

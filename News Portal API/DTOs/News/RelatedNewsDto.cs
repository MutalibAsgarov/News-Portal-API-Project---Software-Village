namespace News_Portal_API.DTOs.News
{
    public class RelatedNewsDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string ShortDescription { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;

        public RelatedNewsCategoryDto? Category { get; set; }
        public RelatedNewsStatsDto? Stats { get; set; }
    }

    public class RelatedNewsCategoryDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? BadgeColor { get; set; }
    }

    public class RelatedNewsStatsDto
    {
        public int LikesCount { get; set; }
        public int SharesCount { get; set; }
        public int BookmarksCount { get; set; }
    }
}
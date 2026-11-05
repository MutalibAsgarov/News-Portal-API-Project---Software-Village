namespace News_Portal_API.DTOs.News
{
    public class NewsDetailDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string ShortDescription { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public int ReadTimeMinutes { get; set; }
        public bool IsTrending { get; set; }
        public bool IsBreaking { get; set; }
        public bool IsLive { get; set; }

        public NewsDetailCategoryDto? Category { get; set; }
        public NewsDetailAuthorDto? Author { get; set; }
        public NewsDetailStatsDto? Stats { get; set; }
    }

    public class NewsDetailCategoryDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public string? BadgeColor { get; set; }
    }

    public class NewsDetailAuthorDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? AvatarUrl { get; set; }
        public string? Bio { get; set; }
        public string? TwitterHandle { get; set; }
    }

    public class NewsDetailStatsDto
    {
        public int LikesCount { get; set; }
        public int SharesCount { get; set; }
        public int BookmarksCount { get; set; }
        public int CommentsCount { get; set; }
    }
}
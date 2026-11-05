namespace News_Portal_API.Entities
{
    public class Article
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string ShortDescription { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public int? CategoryId { get; set; }
        public int? AuthorId { get; set; }
        public int ReadTimeMinutes { get; set; }
        public bool? IsTrending { get; set; }
        public bool? IsBreaking { get; set; }
        public bool? IsLive { get; set; }
        public bool? IsEditorsPick { get; set; }
        public bool? IsFeatured { get; set; }
        public DateTime CreatedAt { get; set; }

        public Category? Category { get; set; }
        public Author? Author { get; set; }
        public ArticleStat? ArticleStat { get; set; }
    }
}

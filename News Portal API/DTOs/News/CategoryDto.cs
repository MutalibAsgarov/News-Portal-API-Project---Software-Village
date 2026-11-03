namespace News_Portal_API.DTOs.News
{
    public class CategoryDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public string? BadgeColor { get; set; }
    }
}

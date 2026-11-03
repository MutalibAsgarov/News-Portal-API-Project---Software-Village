namespace News_Portal_API.DTOs.News
{
    public class LiveNewsDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;

        public LiveCategoryDto? Category { get; set; }
    }

    public class LiveCategoryDto
    {
        public string Name { get; set; } = string.Empty;
    }
}

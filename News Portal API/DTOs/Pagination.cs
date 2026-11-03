namespace News_Portal_API.DTOs
{
    public class Pagination
    {
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int TotalItems { get; set; }
        public int Limit { get; set; }
    }
}

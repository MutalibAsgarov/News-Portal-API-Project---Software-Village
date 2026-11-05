namespace News_Portal_API.Entities
{
    public class ArticleStat
    {
        public int Id { get; set; }
        public int? ArticleId { get; set; }
        public int? LikesCount { get; set; }
        public int? SharesCount { get; set; }
        public int? BookmarksCount { get; set; }
        public int? CommentsCount { get; set; }

        public Article? Article { get; set; }
    }
}

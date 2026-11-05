using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using News_Portal_API.Entities;

namespace News_Portal_API.Configurations;

public class ArticleStatConfiguration : IEntityTypeConfiguration<ArticleStat>
{
    public void Configure(EntityTypeBuilder<ArticleStat> builder)
    {
        builder.ToTable("article_stats");

        builder.Property(s => s.ArticleId)
            .HasColumnName("article_id");

        builder.Property(s => s.LikesCount)
            .HasColumnName("likes_count");

        builder.Property(s => s.SharesCount)
            .HasColumnName("shares_count");

        builder.Property(s => s.BookmarksCount)
            .HasColumnName("bookmarks_count");

        builder.Property(s => s.CommentsCount)
            .HasColumnName("comments_count");

        builder.HasIndex(s => s.ArticleId)
            .IsUnique();
    }
}
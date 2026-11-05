using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using News_Portal_API.Entities;

namespace News_Portal_API.Configurations;

public class ArticleConfiguration : IEntityTypeConfiguration<Article>
{
    public void Configure(EntityTypeBuilder<Article> builder)
    {
        builder.ToTable("articles");

        builder.Property(a => a.ShortDescription)
            .HasColumnName("short_description");

        builder.Property(a => a.ImageUrl)
            .HasColumnName("image_url");

        builder.Property(a => a.CategoryId)
            .HasColumnName("category_id");

        builder.Property(a => a.AuthorId)
            .HasColumnName("author_id");

        builder.Property(a => a.ReadTimeMinutes)
            .HasColumnName("read_time_minutes");

        builder.Property(a => a.IsTrending)
            .HasColumnName("is_trending");

        builder.Property(a => a.IsBreaking)
            .HasColumnName("is_breaking");

        builder.Property(a => a.IsLive)
            .HasColumnName("is_live");

        builder.Property(a => a.IsEditorsPick)
            .HasColumnName("is_editors_pick");

        builder.Property(a => a.IsFeatured)
            .HasColumnName("is_featured");

        builder.Property(a => a.CreatedAt)
            .HasColumnName("created_at");

        builder.HasOne(a => a.Category)
            .WithMany(c => c.Articles)
            .HasForeignKey(a => a.CategoryId);

        builder.HasOne(a => a.Author)
            .WithMany(a => a.Articles)
            .HasForeignKey(a => a.AuthorId);

        builder.HasOne(a => a.ArticleStat)
            .WithOne(s => s.Article)
            .HasForeignKey<ArticleStat>(s => s.ArticleId);
    }
}
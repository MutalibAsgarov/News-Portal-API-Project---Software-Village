using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using News_Portal_API.Entities;

namespace News_Portal_API.Configurations;

public class AuthorConfiguration : IEntityTypeConfiguration<Author>
{
    public void Configure(EntityTypeBuilder<Author> builder)
    {
        builder.ToTable("authors");

        builder.Property(a => a.Name)
            .HasColumnType("varchar(100)");

        builder.Property(a => a.AvatarUrl)
            .HasColumnName("avatar_url")
            .HasColumnType("varchar(500)");

        builder.Property(a => a.Bio)
            .HasColumnType("varchar(max)");

        builder.Property(a => a.TwitterHandle)
            .HasColumnName("twitter_handle")
            .HasColumnType("varchar(50)");
    }
}
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using News_Portal_API.Entities;

namespace News_Portal_API.Configurations;

public class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.ToTable("categories");

        builder.Property(c => c.Name)
            .HasColumnType("varchar(50)");

        builder.Property(c => c.Slug)
            .HasColumnType("varchar(50)");

        builder.Property(c => c.BadgeColor)
            .HasColumnName("badge_color")
            .HasColumnType("varchar(20)");

        builder.HasIndex(c => c.Slug)
            .IsUnique();
    }
}
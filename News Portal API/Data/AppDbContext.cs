using Microsoft.EntityFrameworkCore;

namespace News_Portal_API.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }
}
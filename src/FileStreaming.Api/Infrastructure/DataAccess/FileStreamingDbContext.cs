using Microsoft.EntityFrameworkCore;

namespace FileStreaming.Api.Infrastructure.DataAccess;

public class FileStreamingDbContext(DbContextOptions options) : DbContext(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }
}
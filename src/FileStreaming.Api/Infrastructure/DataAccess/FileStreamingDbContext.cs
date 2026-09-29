using FileStreaming.Api.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FileStreaming.Api.Infrastructure.DataAccess;

public class FileStreamingDbContext(DbContextOptions options) : DbContext(options)
{
    public DbSet<Log> Logs => Set<Log>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(FileStreamingDbContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }
}

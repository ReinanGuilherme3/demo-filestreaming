using FileStreaming.Api.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace FileStreaming.Api.Infrastructure.DataAccess.Repositories;

public class Repository<TEntity>(FileStreamingDbContext context) : IRepository<TEntity> where TEntity : class
{
    protected readonly FileStreamingDbContext Context = context;
    protected readonly DbSet<TEntity> DbSet = context.Set<TEntity>();

    public async Task<TEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        await DbSet.FindAsync([id], cancellationToken);

    public async Task AddAsync(TEntity entity, CancellationToken cancellationToken = default) =>
        await DbSet.AddAsync(entity, cancellationToken);

    public Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default) =>
        DbSet.AddRangeAsync(entities, cancellationToken);

    public void Update(TEntity entity) =>
        DbSet.Update(entity);

    public void Delete(TEntity entity) =>
        DbSet.Remove(entity);
}

using FileStreaming.Api.Domain.Repositories;

namespace FileStreaming.Api.Infrastructure.DataAccess;

public sealed class UnitOfWork(FileStreamingDbContext context) : IUnitOfWork
{
    public Task<int> CommitAsync(CancellationToken cancellationToken = default) =>
        context.SaveChangesAsync(cancellationToken);
}

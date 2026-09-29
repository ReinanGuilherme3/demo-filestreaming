using FileStreaming.Api.Domain.Common;
using FileStreaming.Api.Domain.Entities;
using FileStreaming.Api.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace FileStreaming.Api.Infrastructure.DataAccess.Repositories;

public sealed class LogRepository(FileStreamingDbContext context) : Repository<Log>(context), ILogRepository
{
    public async Task<PagedResult<Log>> GetPaged(int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var query = DbSet.AsNoTracking();

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(log => log.TimestampUtc)
            .ThenByDescending(log => log.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<Log>(items, page, pageSize, totalCount);
    }
}

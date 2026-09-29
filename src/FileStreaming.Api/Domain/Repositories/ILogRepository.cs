using FileStreaming.Api.Domain.Common;
using FileStreaming.Api.Domain.Entities;

namespace FileStreaming.Api.Domain.Repositories;

public interface ILogRepository : IRepository<Log>
{
    Task<PagedResult<Log>> GetPaged(int page, int pageSize, CancellationToken cancellationToken = default);
}

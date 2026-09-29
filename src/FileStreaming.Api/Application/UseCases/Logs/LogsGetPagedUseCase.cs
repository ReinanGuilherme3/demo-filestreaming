using FileStreaming.Api.Domain.Common;
using FileStreaming.Api.Domain.Repositories;

namespace FileStreaming.Api.Application.UseCases.Logs;

public interface ILogsGetPagedUseCase
{
    Task<PagedResult<LogsGetPagedResponse>> Execute(LogsGetPagedQuery query, CancellationToken cancellationToken = default);
}

public class LogsGetPagedUseCase(ILogRepository logRepository) : ILogsGetPagedUseCase
{
    public async Task<PagedResult<LogsGetPagedResponse>> Execute(LogsGetPagedQuery query, CancellationToken cancellationToken = default)
    {
        var logs = await logRepository.GetPaged(query.Page, query.PageSize, cancellationToken);

        return logs.Map(LogsGetPagedResponse.FromEntity);
    }
}

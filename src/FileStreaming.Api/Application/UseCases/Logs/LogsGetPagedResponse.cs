using FileStreaming.Api.Domain.Entities;

namespace FileStreaming.Api.Application.UseCases.Logs;

public record LogsGetPagedResponse(
    Guid Id,
    DateTime TimestampUtc,
    string Method,
    string Path,
    string? QueryString,
    short StatusCode,
    int DurationMs,
    string? RequestBody,
    string? ResponseBody,
    string? ClientMessage,
    string? Exception,
    string? TraceId,
    string? UserId,
    string? ClientIp,
    string? UserAgent)
{
    public static LogsGetPagedResponse FromEntity(Log log) =>
        new(
            Id: log.Id,
            // O SQL Server não guarda o fuso: marca como UTC para o JSON sair com "Z"
            TimestampUtc: DateTime.SpecifyKind(log.TimestampUtc, DateTimeKind.Utc),
            Method: log.Method,
            Path: log.Path,
            QueryString: log.QueryString,
            StatusCode: log.StatusCode,
            DurationMs: log.DurationMs,
            RequestBody: log.RequestBody,
            ResponseBody: log.ResponseBody,
            ClientMessage: log.ClientMessage,
            Exception: log.Exception,
            TraceId: log.TraceId,
            UserId: log.UserId,
            ClientIp: log.ClientIp,
            UserAgent: log.UserAgent);
}

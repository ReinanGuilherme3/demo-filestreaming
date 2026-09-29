namespace FileStreaming.Api.Domain.Entities;

public sealed class Log
{
    public Guid Id { get; init; } = Guid.CreateVersion7();
    public DateTime TimestampUtc { get; init; }
    public string Method { get; init; } = string.Empty;
    public string Path { get; init; } = string.Empty;
    public string? QueryString { get; init; }
    public short StatusCode { get; init; }
    public int DurationMs { get; init; }
    public string? RequestBody { get; init; }
    public string? ResponseBody { get; init; }
    public string? ClientMessage { get; init; }
    public string? Exception { get; init; }
    public string? TraceId { get; init; }
    public string? UserId { get; init; }
    public string? ClientIp { get; init; }
    public string? UserAgent { get; init; }
}

using FluentMigrator;
using TagsAttribute = FluentMigrator.TagsAttribute;

namespace FileStreaming.Api.Infrastructure.Migrations.Versions;

// Roda só em Development (ver AddFluentMigrator) e sem transação única,
// para cada lote ser gravado de forma independente e o log de transação não crescer demais
[Tags(MigrationTags.Development)]
[Migration(DatabaseVersions.SEED_REQUEST_LOGS, TransactionBehavior.None, "Populando tabela de logs com dados de teste")]
public class Version2026092902 : ForwardOnlyMigration
{
    private const int TotalRows = 1_000_000;
    private const int BatchSize = 100_000;

    public override void Up()
    {
        Execute.WithConnection((connection, transaction) =>
        {
            for (var inserted = 0; inserted < TotalRows; inserted += BatchSize)
            {
                using var command = connection.CreateCommand();
                command.Transaction = transaction;
                command.CommandTimeout = 0;
                command.CommandText = InsertBatchSql;

                var parameter = command.CreateParameter();
                parameter.ParameterName = "@BatchSize";
                parameter.Value = Math.Min(BatchSize, TotalRows - inserted);
                command.Parameters.Add(parameter);

                command.ExecuteNonQuery();
            }
        });
    }

    private const string InsertBatchSql = """
        INSERT INTO Logs (Id, TimestampUtc, Method, Path, QueryString, StatusCode, DurationMs,
                          RequestBody, ResponseBody, ClientMessage, Exception, TraceId, UserId, ClientIp, UserAgent)
        SELECT
            NEWID(),
            DATEADD(MILLISECOND, -(Rolls.Seed % 1000), DATEADD(SECOND, -Rolls.SecondsAgo, SYSUTCDATETIME())),
            Routes.Method,
            Routes.Path,
            Routes.QueryString,
            Statuses.StatusCode,
            CASE
                WHEN Statuses.StatusCode = 500 THEN 1000 + Rolls.Seed % 29000
                WHEN Rolls.DurationRoll >= 990 THEN 2000 + Rolls.Seed % 8000
                ELSE 5 + Rolls.DurationRoll % 250
            END,
            Routes.RequestBody,
            CASE
                WHEN Statuses.StatusCode >= 400 THEN CONCAT(N'{"title":"', Statuses.ClientMessage, N'","status":', Statuses.StatusCode, N'}')
                WHEN Statuses.StatusCode IN (204, 302) THEN NULL
                ELSE Routes.ResponseBody
            END,
            Statuses.ClientMessage,
            CASE WHEN Statuses.StatusCode = 500 THEN
                CASE Rolls.Seed % 3
                    WHEN 0 THEN CONCAT(
                        N'Microsoft.Data.SqlClient.SqlException (0x80131904): Execution Timeout Expired. The timeout period elapsed prior to completion of the operation.', CHAR(13), CHAR(10),
                        N'   at Microsoft.Data.SqlClient.SqlCommand.ExecuteReader(CommandBehavior behavior)', CHAR(13), CHAR(10),
                        N'   at FileStreaming.Api.Infrastructure.DataAccess.ExportRepository.StreamAsync(CancellationToken ct)')
                    WHEN 1 THEN CONCAT(
                        N'System.NullReferenceException: Object reference not set to an instance of an object.', CHAR(13), CHAR(10),
                        N'   at FileStreaming.Api.Controllers.ExportsController.Download(Guid id)')
                    ELSE CONCAT(
                        N'Amazon.S3.AmazonS3Exception: "Service Unavailable"; retry later', CHAR(13), CHAR(10),
                        N'   at Amazon.Runtime.Internal.HttpErrorResponseExceptionHandler.HandleException(IExecutionContext context)', CHAR(13), CHAR(10),
                        N'   at FileStreaming.Api.Infrastructure.Storage.R2MultipartUpload.UploadPartAsync(MemoryStream buffer, CancellationToken ct)')
                END
            END,
            LOWER(REPLACE(CONVERT(CHAR(36), NEWID()), '-', '')),
            CASE WHEN Statuses.StatusCode = 401 OR Rolls.Seed % 20 = 0 THEN NULL ELSE CONCAT('user-', Rolls.Seed % 50 + 1) END,
            CONCAT('10.0.', Rolls.Seed % 256, '.', Rolls.Seed / 256 % 256),
            CASE Rolls.Seed % 4
                WHEN 0 THEN N'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/140.0.0.0 Safari/537.36'
                WHEN 1 THEN N'Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:143.0) Gecko/20100101 Firefox/143.0'
                WHEN 2 THEN N'PostmanRuntime/7.45.0'
                ELSE N'curl/8.9.1'
            END
        FROM (
            SELECT TOP (@BatchSize) ROW_NUMBER() OVER (ORDER BY (SELECT NULL)) AS N
            FROM sys.all_objects AS a
            CROSS JOIN sys.all_objects AS b
        ) AS Numbers
        CROSS APPLY (
            SELECT
                Numbers.N AS N,
                ABS(CHECKSUM(NEWID())) % 100 AS RouteRoll,
                ABS(CHECKSUM(NEWID())) % 100 AS StatusRoll,
                ABS(CHECKSUM(NEWID())) % 1000 AS DurationRoll,
                ABS(CHECKSUM(NEWID())) % 7776000 AS SecondsAgo, -- até 90 dias atrás
                ABS(CHECKSUM(NEWID())) AS Seed,
                CONVERT(NVARCHAR(36), NEWID()) AS ResourceId
        ) AS Rolls
        CROSS APPLY (
            SELECT
                Route.Method,
                Route.Path,
                Route.QueryString,
                Route.RequestBody,
                Route.ResponseBody
            FROM (VALUES
                (0,  35, 'GET',    N'/logs', CONCAT(N'?page=', Rolls.Seed % 100 + 1, N'&pageSize=50'), NULL, N'{"items":[],"page":1,"pageSize":50}'),
                (35, 55, 'GET',    N'/exports', N'?page=1', NULL, N'[{"id":"' + Rolls.ResourceId + N'","status":"Completed"}]'),
                (55, 70, 'GET',    CONCAT(N'/exports/', Rolls.ResourceId), NULL, NULL, N'{"id":"' + Rolls.ResourceId + N'","status":"Processing","processedRows":50000}'),
                (70, 80, 'POST',   N'/exports', NULL, N'{"filters":{"statusCode":500,"from":"2026-09-01T00:00:00Z"}}', N'{"id":"' + Rolls.ResourceId + N'"}'),
                (80, 86, 'GET',    CONCAT(N'/exports/', Rolls.ResourceId, N'/download'), NULL, NULL, NULL),
                (86, 90, 'POST',   CONCAT(N'/exports/', Rolls.ResourceId, N'/cancel'), NULL, NULL, N'{"status":"Cancelled"}'),
                (90, 97, 'PUT',    CONCAT(N'/users/', Rolls.Seed % 50 + 1), NULL,
                                   CONCAT(N'{"name":"Cliente ', Rolls.Seed % 50 + 1, N'; \"Silva\"","email":"cliente', Rolls.Seed % 50 + 1, N'@teste.com"}'),
                                   N'{"updated":true}'),
                (97, 100, 'DELETE', CONCAT(N'/users/', Rolls.Seed % 50 + 1), NULL, NULL, NULL)
            ) AS Route (RollFrom, RollTo, Method, Path, QueryString, RequestBody, ResponseBody)
            WHERE Rolls.RouteRoll >= Route.RollFrom AND Rolls.RouteRoll < Route.RollTo
        ) AS Routes
        CROSS APPLY (
            SELECT
                CASE
                    WHEN Rolls.StatusRoll < 80 THEN
                        CASE
                            WHEN Routes.Method = 'POST' AND Routes.Path = N'/exports' THEN 202
                            WHEN Routes.Method = 'DELETE' THEN 204
                            WHEN Routes.Path LIKE N'%/download' THEN 302
                            ELSE 200
                        END
                    WHEN Rolls.StatusRoll < 88 THEN 400
                    WHEN Rolls.StatusRoll < 94 THEN 404
                    WHEN Rolls.StatusRoll < 97 THEN 401
                    ELSE 500
                END AS StatusCode
        ) AS StatusCodes
        CROSS APPLY (
            SELECT
                StatusCodes.StatusCode,
                CASE StatusCodes.StatusCode
                    WHEN 400 THEN N'Os dados enviados são inválidos.'
                    WHEN 401 THEN N'Usuário não autenticado.'
                    WHEN 404 THEN N'Recurso não encontrado.'
                    WHEN 500 THEN N'Ocorreu um erro inesperado. Tente novamente mais tarde.'
                END AS ClientMessage
        ) AS Statuses;
        """;
}

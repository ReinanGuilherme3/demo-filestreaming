using FluentMigrator;
using FluentMigrator.SqlServer;

namespace FileStreaming.Api.Infrastructure.Migrations.Versions;

[Migration(DatabaseVersions.TABLE_REQUEST_LOGS, "Criando tabela de auditoria das requisições")]
public class Version2026092901 : ForwardOnlyMigration
{
    private const string TableName = "Logs";

    public override void Up()
    {
        Create.Table(TableName)
            .WithColumn("Id").AsGuid().NotNullable()
            .WithColumn("TimestampUtc").AsDateTime2().NotNullable()
            .WithColumn("Method").AsAnsiString(10).NotNullable()
            .WithColumn("Path").AsString(2000).NotNullable()
            .WithColumn("QueryString").AsString(2000).Nullable()
            .WithColumn("StatusCode").AsInt16().NotNullable()
            .WithColumn("DurationMs").AsInt32().NotNullable()
            .WithColumn("RequestBody").AsString(int.MaxValue).Nullable()
            .WithColumn("ResponseBody").AsString(int.MaxValue).Nullable()
            .WithColumn("ClientMessage").AsString(1000).Nullable()
            .WithColumn("Exception").AsString(int.MaxValue).Nullable()
            .WithColumn("TraceId").AsFixedLengthAnsiString(32).Nullable()
            .WithColumn("UserId").AsString(100).Nullable()
            .WithColumn("ClientIp").AsAnsiString(45).Nullable()
            .WithColumn("UserAgent").AsString(500).Nullable();

        // GUID v7 não é ordenado pelo SQL Server, por isso a PK não é clusterizada
        Create.PrimaryKey("PK_Logs").OnTable(TableName)
            .Column("Id")
            .NonClustered();

        Create.Index("CIX_Logs_Timestamp").OnTable(TableName)
            .OnColumn("TimestampUtc").Descending()
            .OnColumn("Id").Descending()
            .WithOptions().Unique()
            .WithOptions().Clustered();

        Create.Index("IX_Logs_StatusCode").OnTable(TableName)
            .OnColumn("StatusCode").Ascending()
            .OnColumn("TimestampUtc").Descending();

        Create.Index("IX_Logs_UserId").OnTable(TableName)
            .OnColumn("UserId").Ascending()
            .OnColumn("TimestampUtc").Descending();
    }
}

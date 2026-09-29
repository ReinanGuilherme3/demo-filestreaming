using FileStreaming.Api.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FileStreaming.Api.Infrastructure.DataAccess.Mappings;

public sealed class LogMap : IEntityTypeConfiguration<Log>
{
    public void Configure(EntityTypeBuilder<Log> builder)
    {
        builder.ToTable("Logs");

        // O schema é criado pelo FluentMigrator; aqui só descrevemos a tabela para o EF
        builder.HasKey(log => log.Id)
            .IsClustered(false);

        builder.Property(log => log.Id)
            .ValueGeneratedNever();

        builder.Property(log => log.TimestampUtc)
            .HasColumnType("datetime2")
            .IsRequired();

        builder.Property(log => log.Method)
            .HasMaxLength(10)
            .IsUnicode(false)
            .IsRequired();

        builder.Property(log => log.Path)
            .HasMaxLength(2000)
            .IsRequired();

        builder.Property(log => log.QueryString)
            .HasMaxLength(2000);

        builder.Property(log => log.StatusCode)
            .IsRequired();

        builder.Property(log => log.DurationMs)
            .IsRequired();

        builder.Property(log => log.RequestBody);

        builder.Property(log => log.ResponseBody);

        builder.Property(log => log.ClientMessage)
            .HasMaxLength(1000);

        builder.Property(log => log.Exception);

        builder.Property(log => log.TraceId)
            .HasMaxLength(32)
            .IsFixedLength()
            .IsUnicode(false);

        builder.Property(log => log.UserId)
            .HasMaxLength(100);

        builder.Property(log => log.ClientIp)
            .HasMaxLength(45)
            .IsUnicode(false);

        builder.Property(log => log.UserAgent)
            .HasMaxLength(500);
    }
}

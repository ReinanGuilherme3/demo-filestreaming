using FileStreaming.Api.Infrastructure.DataAccess;
using FluentMigrator.Runner;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace FileStreaming.Api.Configurations;

public static class InfrastructureConfiguration
{
    private static void AddDbContext(IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Connection");

        services.AddDbContext<FileStreamingDbContext>(dbContextOptions =>
        {
            dbContextOptions.UseSqlServer(connectionString);
        });
    }

    private static void AddFluentMigrator(IServiceCollection services, IConfiguration configuration)
    {
        var infrastructureAssembly = Assembly.Load("FluentMigratorDemo.Infrastructure");

        var connectionString = configuration.GetConnectionString("Connection");

        services.AddFluentMigratorCore().ConfigureRunner(config =>
        {
            var migrationRunnerBuilder = config.AddSqlServer();

            migrationRunnerBuilder
            .WithGlobalConnectionString(connectionString)
            .ScanIn(infrastructureAssembly)
            .For.All();
        });
    }
}

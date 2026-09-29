using FileStreaming.Api.Domain.Repositories;
using FileStreaming.Api.Infrastructure.DataAccess;
using FileStreaming.Api.Infrastructure.DataAccess.Repositories;
using FileStreaming.Api.Infrastructure.Migrations;
using FileStreaming.Api.Settings;
using FluentMigrator.Runner;
using FluentMigrator.Runner.Initialization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace FileStreaming.Api.Configurations;

public static class InfrastructureConfiguration
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddDbContext()
                .AddRepositories()
                .AddFluentMigrator();

        return services;
    }

    public static WebApplication MigrateDatabase(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();

        var connectionStringsSettings = scope.ServiceProvider.GetRequiredService<IOptions<ConnectionStringsSettings>>().Value;

        DatabaseMigration.Migrate(connectionStringsSettings.ConnectionString, scope.ServiceProvider);

        return app;
    }

    private static IServiceCollection AddDbContext(this IServiceCollection services)
    {
        services.AddDbContext<FileStreamingDbContext>((serviceProvider, dbContextOptions) =>
        {
            var connectionStringsSettings = serviceProvider.GetRequiredService<IOptions<ConnectionStringsSettings>>().Value;

            dbContextOptions.UseSqlServer(connectionStringsSettings.ConnectionString);
        });

        return services;
    }

    private static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<ILogRepository, LogRepository>();

        return services;
    }

    private static IServiceCollection AddFluentMigrator(this IServiceCollection services)
    {
        var infrastructureAssembly = typeof(DatabaseMigration).Assembly;

        services.AddFluentMigratorCore().ConfigureRunner(config =>
        {
            var migrationRunnerBuilder = config.AddSqlServer();

            migrationRunnerBuilder
            .WithGlobalConnectionString(serviceProvider =>
                serviceProvider.GetRequiredService<IOptions<ConnectionStringsSettings>>().Value.ConnectionString)
            .ScanIn(infrastructureAssembly)
            .For.All();
        });

        // Migrations com a tag Development (seed de dados) só rodam nesse ambiente
        services.AddOptions<RunnerOptions>()
            .Configure<IHostEnvironment>((runnerOptions, environment) =>
            {
                runnerOptions.IncludeUntaggedMigrations = true;

                if (environment.IsDevelopment())
                    runnerOptions.Tags = [MigrationTags.Development];
            });

        return services;
    }
}

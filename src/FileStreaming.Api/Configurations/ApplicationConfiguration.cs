using FileStreaming.Api.Application.UseCases.Logs;
using FluentValidation;

namespace FileStreaming.Api.Configurations;

public static class ApplicationConfiguration
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddUseCases()
                .AddValidators();

        return services;
    }

    private static IServiceCollection AddValidators(this IServiceCollection services)
    {
        services.AddValidatorsFromAssemblyContaining<LogsGetPagedQueryValidator>();

        return services;
    }

    private static IServiceCollection AddUseCases(this IServiceCollection services)
    {
        services.AddScoped<ILogsGetPagedUseCase, LogsGetPagedUseCase>();

        return services;
    }
}

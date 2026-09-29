using FileStreaming.Api.Application.UseCases.Logs;

namespace FileStreaming.Api.Configurations;

public static class ApplicationConfiguration
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddUseCases();

        return services;
    }

    private static IServiceCollection AddUseCases(this IServiceCollection services)
    {
        services.AddScoped<ILogsGetPagedUseCase, LogsGetPagedUseCase>();

        return services;
    }
}

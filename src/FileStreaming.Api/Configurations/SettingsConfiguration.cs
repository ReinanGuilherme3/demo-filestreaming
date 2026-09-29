using FileStreaming.Api.Settings;
using Microsoft.Extensions.Options;

namespace FileStreaming.Api.Configurations;

public static class SettingsConfiguration
{
    public static IServiceCollection AddSettings(this IServiceCollection services)
    {
        services.AddValidatedSettings<ConnectionStringsSettings>();

        return services;
    }

    private static OptionsBuilder<TSettings> AddValidatedSettings<TSettings>(this IServiceCollection services)
        where TSettings : class, ISettings =>
        services.AddOptions<TSettings>()
            .BindConfiguration(TSettings.SectionName)
            .ValidateDataAnnotations()
            .ValidateOnStart();
}

namespace FileStreaming.Api.Configurations;

public static class ApiConfiguration
{
    public static IServiceCollection AddApi(this IServiceCollection services)
    {
        services.AddControllers();

        services.AddRouting(options => options.LowercaseUrls = true);

        return services;

    }

    public static WebApplication UseApi(this WebApplication app)
    {
        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.MapControllers();

        return app;
    }
}

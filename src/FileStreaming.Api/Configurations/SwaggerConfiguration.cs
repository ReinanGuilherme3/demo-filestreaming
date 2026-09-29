namespace FileStreaming.Api.Configurations;

public static class SwaggerConfiguration
{
    private const string DocumentName = "v1";
    private const string RoutePrefix = "";

    public static IServiceCollection AddSwagger(this IServiceCollection services)
    {
        services.AddOpenApi(DocumentName);

        return services;
    }

    public static WebApplication UseSwagger(this WebApplication app)
    {
        if (!app.Environment.IsDevelopment())
            return app;

        // Documento gerado pelo próprio .NET em /openapi/v1.json
        app.MapOpenApi();

        // Swagger UI em /docs, lendo o documento acima
        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint($"/openapi/{DocumentName}.json", "FileStreaming API v1");
            options.RoutePrefix = RoutePrefix;
        });

        return app;
    }
}

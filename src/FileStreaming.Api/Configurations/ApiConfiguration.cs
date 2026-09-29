using FileStreaming.Api.Filters;
using FileStreaming.Api.Handlers;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace FileStreaming.Api.Configurations;

public static class ApiConfiguration
{
    public static IServiceCollection AddApi(this IServiceCollection services)
    {
        services.AddControllers(options => options.Filters.Add<ValidationFilter>());

        services.AddRouting(options => options.LowercaseUrls = true);

        services.AddErrorHandling();

        return services;
    }

    public static WebApplication UseApi(this WebApplication app)
    {
        // Primeiro no pipeline, para capturar exceções de tudo que vem depois
        app.UseExceptionHandler();

        // Respostas de erro sem corpo (ex.: 404 de rota inexistente) também saem como ProblemDetails
        app.UseStatusCodePages();

        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.MapControllers();

        return app;
    }

    private static IServiceCollection AddErrorHandling(this IServiceCollection services)
    {
        // Todo ProblemDetails da aplicação passa por aqui: validação, Result, exceções e status codes
        services.AddProblemDetails(options => options.CustomizeProblemDetails = context =>
        {
            var problemDetails = context.ProblemDetails;
            var httpContext = context.HttpContext;

            problemDetails.Title = problemDetails.Status switch
            {
                StatusCodes.Status400BadRequest => "Dados inválidos.",
                StatusCodes.Status404NotFound => "Recurso não encontrado.",
                StatusCodes.Status409Conflict => "Conflito com o estado atual do recurso.",
                StatusCodes.Status410Gone => "Recurso não está mais disponível.",
                StatusCodes.Status500InternalServerError => "Erro interno.",
                _ => problemDetails.Title
            };

            if (problemDetails is ValidationProblemDetails)
                problemDetails.Detail ??= "Um ou mais campos não passaram na validação.";

            problemDetails.Instance = $"{httpContext.Request.Method} {httpContext.Request.Path}";

            // Mesmo formato do TraceId gravado na tabela Logs (32 caracteres hexadecimais)
            var activity = httpContext.Features.Get<IHttpActivityFeature>()?.Activity ?? Activity.Current;
            problemDetails.Extensions["traceId"] = activity?.TraceId.ToString() ?? httpContext.TraceIdentifier;
        });

        services.AddExceptionHandler<GlobalExceptionHandler>();

        return services;
    }
}

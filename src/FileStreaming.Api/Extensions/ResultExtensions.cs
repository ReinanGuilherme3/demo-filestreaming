using FileStreaming.Api.Application.Common.Results;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace FileStreaming.Api.Extensions;

public static class ResultExtensions
{
    public static IActionResult ToActionResult<TValue>(this Result<TValue> result, ControllerBase controller) =>
        result.IsSuccess
            ? controller.Ok(result.Value)
            : result.Error!.ToProblem(controller.HttpContext);

    public static IActionResult ToActionResult(this Result result, ControllerBase controller) =>
        result.IsSuccess
            ? controller.NoContent()
            : result.Error!.ToProblem(controller.HttpContext);

    private static ObjectResult ToProblem(this Error error, HttpContext httpContext)
    {
        var statusCode = error.Type switch
        {
            ErrorType.Validation => StatusCodes.Status400BadRequest,
            ErrorType.NotFound => StatusCodes.Status404NotFound,
            ErrorType.Conflict => StatusCodes.Status409Conflict,
            ErrorType.Gone => StatusCodes.Status410Gone,
            _ => StatusCodes.Status500InternalServerError
        };

        // A fábrica aplica o CustomizeProblemDetails (traceId, instance), igual aos outros erros
        var problemDetailsFactory = httpContext.RequestServices.GetRequiredService<ProblemDetailsFactory>();

        var problemDetails = problemDetailsFactory.CreateProblemDetails(
            httpContext,
            statusCode: statusCode,
            detail: error.Message);

        problemDetails.Extensions["code"] = error.Code;

        return new ObjectResult(problemDetails) { StatusCode = statusCode };
    }
}

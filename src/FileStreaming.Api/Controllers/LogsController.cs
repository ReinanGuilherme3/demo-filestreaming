using FileStreaming.Api.Application.UseCases.Logs;
using FileStreaming.Api.Domain.Common;
using FileStreaming.Api.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace FileStreaming.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
[ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError, "application/problem+json")]
public class LogsController : ControllerBase
{
    [HttpGet]
    [ProducesResponseType<PagedResult<LogsGetPagedResponse>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest, "application/problem+json")]
    public async Task<IActionResult> GetPaged(
        [FromQuery] LogsGetPagedQuery query,
        [FromServices] ILogsGetPagedUseCase useCase,
        CancellationToken cancellationToken)
    {
        var result = await useCase.Execute(query, cancellationToken);

        return result.ToActionResult(this);
    }
}

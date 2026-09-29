using FileStreaming.Api.Application.UseCases.Logs;
using FileStreaming.Api.Domain.Common;
using Microsoft.AspNetCore.Mvc;

namespace FileStreaming.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class LogsController : ControllerBase
{
    [HttpGet]
    [ProducesResponseType<PagedResult<LogsGetPagedResponse>>(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPaged(
        [FromQuery] LogsGetPagedQuery query,
        [FromServices] ILogsGetPagedUseCase useCase,
        CancellationToken cancellationToken)
    {
        var logs = await useCase.Execute(query, cancellationToken);

        return Ok(logs);
    }
}

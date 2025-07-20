using Fermion.EntityFramework.Identity.Application.DTOs.ApplicationUserSessions;
using Fermion.EntityFramework.Identity.Domain.Interfaces.Services;
using Fermion.EntityFramework.Shared.DTOs.Pagination;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Fermion.EntityFramework.Identity.Presentation.Controllers;

[ApiController]
[Route("api/user-sessions")]
public class ApplicationUserSessionController(
    IApplicationUserSessionAppService userSessionAppService)
    : ControllerBase
{
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ApplicationUserSessionResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetByIdAsync([FromRoute(Name = "id")] Guid id, CancellationToken cancellationToken = default)
    {
        var result = await userSessionAppService.GetByIdAsync(id, cancellationToken);
        return Ok(result);
    }

    [HttpGet("pageable")]
    [ProducesResponseType(typeof(PageableResponseDto<ApplicationUserSessionResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetPageableAndFilterAsync([FromQuery] GetListApplicationUserSessionRequestDto request, CancellationToken cancellationToken = default)
    {
        var result = await userSessionAppService.GetPageableAndFilterAsync(request, cancellationToken);
        return Ok(result);
    }
}
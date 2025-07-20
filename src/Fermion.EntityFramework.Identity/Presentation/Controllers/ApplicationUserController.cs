using Fermion.EntityFramework.Identity.Application.DTOs.ApplicationUsers;
using Fermion.EntityFramework.Identity.Domain.Interfaces.Services;
using Fermion.EntityFramework.Shared.DTOs.Pagination;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Fermion.EntityFramework.Identity.Presentation.Controllers;

[ApiController]
[Route("api/users")]
public class ApplicationUserController(
    IApplicationUserAppService userAppService) 
    : ControllerBase
{
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApplicationUserResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> GetByIdAsync([FromRoute(Name = "id")] Guid id, CancellationToken cancellationToken = default)
    {
        var result = await userAppService.GetByIdAsync(id, cancellationToken);
        return Ok(result);
    }

    [HttpGet("pageable")]
    [ProducesResponseType(typeof(PageableResponseDto<ApplicationUserResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> GetPageableAndFilterAsync([FromQuery] GetListApplicationUserRequestDto request, CancellationToken cancellationToken = default)
    {
        var result = await userAppService.GetPageableAndFilterAsync(request, cancellationToken);
        return Ok(result);
    }

    [HttpPost]
    [ProducesResponseType(typeof(ApplicationUserResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> CreateAsync([FromBody] CreateApplicationUserRequestDto request, CancellationToken cancellationToken = default)
    {
        var result = await userAppService.CreateAsync(request, cancellationToken);
        return Ok(result);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ApplicationUserResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> UpdateAsync([FromRoute(Name = "id")] Guid id, [FromBody] UpdateApplicationUserRequestDto request, CancellationToken cancellationToken = default)
    {
        var result = await userAppService.UpdateAsync(id, request, cancellationToken);
        return Ok(result);
    }
    
    [HttpPost("change-password")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> ChangePasswordAsync([FromBody] ChangePasswordApplicationUserRequestDto request, CancellationToken cancellationToken = default)
    {
        await userAppService.ChangePasswordAsync(request, cancellationToken);
        return NoContent();
    }
    
    [HttpPost("{id:guid}/lock")]
    [ProducesResponseType(typeof(ApplicationUserResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> LockAsync([FromRoute(Name = "id")] Guid id, [FromQuery] bool isPermanent, CancellationToken cancellationToken = default)
    {
        var result = await userAppService.LockAsync(id, isPermanent, cancellationToken);
        return Ok(result);
    }
    
    [HttpPost("{id:guid}/unlock")]
    [ProducesResponseType(typeof(ApplicationUserResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> UnlockAsync([FromRoute(Name = "id")] Guid id, CancellationToken cancellationToken = default)
    {
        var result = await userAppService.UnlockAsync(id, cancellationToken);
        return Ok(result);
    }
    
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> DeleteAsync([FromRoute(Name = "id")] Guid id, CancellationToken cancellationToken = default)
    {
        await userAppService.DeleteAsync(id, cancellationToken);
        return NoContent();
    }
}
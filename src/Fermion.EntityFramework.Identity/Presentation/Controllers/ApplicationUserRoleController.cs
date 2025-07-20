using Fermion.EntityFramework.Identity.Application.DTOs.ApplicationRoles;
using Fermion.EntityFramework.Identity.Application.DTOs.ApplicationUsers;
using Fermion.EntityFramework.Identity.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Fermion.EntityFramework.Identity.Presentation.Controllers;

[ApiController]
[Route("api/user-roles")]
public class ApplicationUserRoleController(
    IApplicationUserRoleAppService userRoleAppService) 
    : ControllerBase
{
    [HttpGet("roles/{userId:guid}")]
    [ProducesResponseType(typeof(List<ApplicationRoleResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> GetRolesByUserIdAsync([FromRoute(Name = "userId")] Guid userId, CancellationToken cancellationToken = default)
    {
        var result = await userRoleAppService.GetRolesByUserIdAsync(userId, cancellationToken);
        return Ok(result);
    }
    
    [HttpGet("users/{roleId:guid}")]
    [ProducesResponseType(typeof(List<ApplicationUserResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> GetUsersByRoleIdAsync([FromRoute(Name = "roleId")] Guid roleId, CancellationToken cancellationToken = default)
    {
        var result = await userRoleAppService.GetUsersByRoleIdAsync(roleId, cancellationToken);
        return Ok(result);
    }
    
    [HttpPost("assign/{roleId:guid}/to/{userId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> AssignRoleToUserAsync(
        [FromRoute(Name = "roleId")] Guid roleId,
        [FromRoute(Name = "userId")] Guid userId,
        CancellationToken cancellationToken = default)
    {
        await userRoleAppService.AssignRoleToUserAsync(userId, roleId, cancellationToken);
        return NoContent();
    }
    
    [HttpDelete("remove/{roleId:guid}/from/{userId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> RemoveRoleFromUserAsync(
        [FromRoute(Name = "roleId")] Guid roleId,
        [FromRoute(Name = "userId")] Guid userId,
        CancellationToken cancellationToken = default)
    {
        await userRoleAppService.RemoveRoleFromUserAsync(userId, roleId, cancellationToken);
        return NoContent();
    }
    
    [HttpDelete("clear/{userId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> ClearRolesFromUserAsync([FromRoute(Name = "userId")] Guid userId, CancellationToken cancellationToken = default)
    {
        await userRoleAppService.ClearRolesFromUserAsync(userId, cancellationToken);
        return NoContent();
    }
}
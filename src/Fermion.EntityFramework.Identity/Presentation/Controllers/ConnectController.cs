using Fermion.EntityFramework.Identity.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Server.AspNetCore;

namespace Fermion.EntityFramework.Identity.Presentation.Controllers;

[ApiController]
[Route("connect")]
public class ConnectController(
    IAccountAppService accountAppService)
    : ControllerBase
{
    [HttpPost("token")]
    public async Task<IActionResult> LoginAsync()
    {
        var claimsPrincipal = await accountAppService.TokenAsync();
        return SignIn(claimsPrincipal, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
    }
}
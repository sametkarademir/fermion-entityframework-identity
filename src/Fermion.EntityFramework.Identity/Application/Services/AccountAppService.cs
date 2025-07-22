using System.Collections.Immutable;
using System.Security.Claims;
using Fermion.Domain.Exceptions.Types;
using Fermion.Domain.Extensions.HttpContexts;
using Fermion.EntityFramework.Identity.Domain.Entities;
using Fermion.EntityFramework.Identity.Domain.Interfaces.Repositories;
using Fermion.EntityFramework.Identity.Domain.Interfaces.Services;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using OpenIddict.Abstractions;
using OpenIddict.Server.AspNetCore;

namespace Fermion.EntityFramework.Identity.Application.Services;

public class AccountAppService(
    UserManager<ApplicationUser> userManager,
    SignInManager<ApplicationUser> signInManager,
    IApplicationUserSessionRepository userSessionRepository,
    IHttpContextAccessor httpContextAccessor)
    : IAccountAppService
{
    public async Task<ClaimsPrincipal> TokenAsync(CancellationToken cancellationToken = default)
    {
        var request = httpContextAccessor.HttpContext?.GetOpenIddictServerRequest();
        if (request is null)
        {
            throw new AppAuthenticationException("The OpenIddict request cannot be retrieved.");
        }

        if (request.IsPasswordGrantType())
        {
            return await LoginAsync(cancellationToken);
        }

        if (request.IsRefreshTokenGrantType())
        {
            return await RefreshTokenAsync(cancellationToken);
        }

        throw new AppAuthenticationException("The password or refresh token grant type is not supported.");
    }

    private async Task<ClaimsPrincipal> LoginAsync(CancellationToken cancellationToken = default)
    {
        var request = httpContextAccessor.HttpContext?.GetOpenIddictServerRequest();
        if (request is null)
        {
            throw new AppAuthenticationException("The OpenIddict request cannot be retrieved.");
        }

        if (!request.IsPasswordGrantType())
        {
            throw new AppAuthenticationException("The password grant type is not supported.");
        }

        var matchedUser = await userManager.FindByNameAsync(request.Username!);
        if (matchedUser is null)
        {
            throw new AppAuthenticationException("Invalid credentials.");
        }

        if (!matchedUser.EmailConfirmed)
        {
            throw new AppAuthenticationException("Email not confirmed.");
        }

        if (matchedUser.LockoutEnd.HasValue && matchedUser.LockoutEnd.Value > DateTimeOffset.UtcNow)
        {
            throw new AppAuthenticationException("Account is locked out.");
        }

        var result = await signInManager.CheckPasswordSignInAsync(matchedUser, request.Password!, false);
        if (result.IsLockedOut)
        {
            throw new AppAuthenticationException("Account is locked out.");
        }

        if (!result.Succeeded)
        {
            await userManager.AccessFailedAsync(matchedUser);
            throw new AppAuthenticationException("Invalid credentials.");
        }

        await userManager.ResetAccessFailedCountAsync(matchedUser);
        await userManager.SetLockoutEndDateAsync(matchedUser, null);

        if (httpContextAccessor.HttpContext == null)
        {
            throw new AppBusinessException("HTTP context is not available.");
        }

        var deviceInfo = httpContextAccessor.HttpContext.GetDeviceInfo();
        var newUserSession = new ApplicationUserSession
        {
            ClientIp = httpContextAccessor.HttpContext.GetClientIpAddress(),
            UserAgent = httpContextAccessor.HttpContext.GetUserAgent(),
            DeviceFamily = deviceInfo.DeviceFamily,
            DeviceModel = deviceInfo.DeviceModel,
            OsFamily = deviceInfo.OsFamily,
            OsVersion = deviceInfo.OsVersion,
            BrowserFamily = deviceInfo.BrowserFamily,
            BrowserVersion = deviceInfo.BrowserVersion,
            IsMobile = deviceInfo.IsMobile,
            IsDesktop = deviceInfo.IsDesktop,
            IsTablet = deviceInfo.IsTablet,
            UserId = matchedUser.Id
        };
        newUserSession = await userSessionRepository.AddAsync(newUserSession, cancellationToken: cancellationToken);

        var identity = await CreateIdentityAsync(matchedUser, newUserSession.Id, request.GetScopes());
        var principal = new ClaimsPrincipal(identity);

        return principal;
    }

    private async Task<ClaimsPrincipal> RefreshTokenAsync(CancellationToken cancellationToken = default)
    {
        var request = httpContextAccessor.HttpContext?.GetOpenIddictServerRequest();
        if (request is null)
        {
            throw new AppAuthenticationException("The OpenIddict request cannot be retrieved.");
        }

        if (!request.IsRefreshTokenGrantType())
        {
            throw new AppAuthenticationException("The password grant type is not supported.");
        }

        var principal = (await httpContextAccessor.HttpContext!.AuthenticateAsync(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme)).Principal!;
        var matchedUserId = principal.GetClaim(OpenIddictConstants.Claims.Subject);

        if (matchedUserId is null)
        {
            throw new AppAuthenticationException("Invalid credentials.");
        }

        var matchedUser = await userManager.FindByIdAsync(matchedUserId);
        if (matchedUser is null)
        {
            throw new AppAuthenticationException("Invalid credentials.");
        }

        var currentSessionId = httpContextAccessor.HttpContext!.GetSessionId();
        if (currentSessionId is null)
        {
            throw new AppAuthenticationException("Invalid session.");
        }

        var identity = await CreateIdentityAsync(matchedUser, currentSessionId.Value, request.GetScopes());
        var newPrincipal = new ClaimsPrincipal(identity);

        return newPrincipal;
    }

    private async Task<ClaimsIdentity> CreateIdentityAsync(ApplicationUser user, Guid sessionId, ImmutableArray<string> scopes)
    {
        var identity = new ClaimsIdentity(
            authenticationType: TokenValidationParameters.DefaultAuthenticationType,
            nameType: OpenIddictConstants.Claims.Name,
            roleType: OpenIddictConstants.Claims.Role);

        identity.AddClaim(new Claim(OpenIddictConstants.Claims.Subject, user.Id.ToString()));

        identity
            .AddClaim(new Claim(ClaimTypes.Name, user.UserName!)
            .SetDestinations(OpenIddictConstants.Destinations.AccessToken));
        identity
            .AddClaim(new Claim(ClaimTypes.Email, user.Email!)
            .SetDestinations(OpenIddictConstants.Destinations.AccessToken));
        identity
            .AddClaim(new Claim("X-Session-ID", sessionId.ToString())
            .SetDestinations(OpenIddictConstants.Destinations.AccessToken));
        identity
            .AddClaim(new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
            .SetDestinations(OpenIddictConstants.Destinations.AccessToken));
        
        if (scopes.Contains(OpenIddictConstants.Scopes.Roles))
        {
            var userRoles = await userManager.GetRolesAsync(user);
            foreach (var role in userRoles)
            {
                identity.AddClaim(new Claim(ClaimTypes.Role, role)
                    .SetDestinations(OpenIddictConstants.Destinations.AccessToken));
            }
        }

        identity.SetScopes(scopes);
        identity.SetResources("api");

        return identity;
    }
}
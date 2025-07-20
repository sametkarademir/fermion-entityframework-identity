using System.Security.Claims;
using Fermion.Domain.Exceptions.Types;
using Fermion.Domain.Extensions.Claims;
using Fermion.EntityFramework.Identity.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Http;

namespace Fermion.EntityFramework.Identity.Application.Services;

public class CurrentUser: ICurrentUser
{
    private readonly IHttpContextAccessor _httpContextAccessor;
   
    public CurrentUser(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
        if (_httpContextAccessor.HttpContext == null)
        {
            throw new AppBusinessException("HttpContext is not available. Ensure that the IHttpContextAccessor is properly configured in the application.");
        }
    }

    public bool IsAuthenticated => _httpContextAccessor.HttpContext!.User.Identity?.IsAuthenticated ?? false;
    public Guid? Id => _httpContextAccessor.HttpContext!.User.GetUserIdToGuid();
    public string? UserName => _httpContextAccessor.HttpContext!.User.GetUserName();
    public string? Email => _httpContextAccessor.HttpContext!.User.GetUserEmail();
    public List<string>? Roles => _httpContextAccessor.HttpContext!.User.GetUserRoles();
    public bool IsInRole(string role) => _httpContextAccessor.HttpContext!.User.IsInRole(role);
    public ClaimsPrincipal User() => _httpContextAccessor.HttpContext!.User;
}
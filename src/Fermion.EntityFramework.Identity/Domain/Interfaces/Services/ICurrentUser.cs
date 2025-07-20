using System.Security.Claims;

namespace Fermion.EntityFramework.Identity.Domain.Interfaces.Services;

public interface ICurrentUser
{
    bool IsAuthenticated { get; }
    Guid? Id { get; }
    string? UserName { get; }
    string? Email { get; }
    List<string>? Roles { get; }
    bool IsInRole(string role);

    ClaimsPrincipal User();
}
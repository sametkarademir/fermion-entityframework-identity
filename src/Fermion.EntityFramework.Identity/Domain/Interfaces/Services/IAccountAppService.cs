using System.Security.Claims;

namespace Fermion.EntityFramework.Identity.Domain.Interfaces.Services;

public interface IAccountAppService
{
    Task<ClaimsPrincipal> TokenAsync(CancellationToken cancellationToken = default);
}
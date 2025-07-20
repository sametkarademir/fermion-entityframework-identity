using Fermion.EntityFramework.Identity.Application.DTOs.ApplicationUserSessions;
using Fermion.EntityFramework.Shared.DTOs.Pagination;

namespace Fermion.EntityFramework.Identity.Domain.Interfaces.Services;

public interface IApplicationUserSessionAppService
{
    Task<ApplicationUserSessionResponseDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<PageableResponseDto<ApplicationUserSessionResponseDto>> GetPageableAndFilterAsync(GetListApplicationUserSessionRequestDto request, CancellationToken cancellationToken = default);
}
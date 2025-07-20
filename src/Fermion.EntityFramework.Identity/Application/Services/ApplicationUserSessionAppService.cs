using AutoMapper;
using Fermion.Domain.Extensions.Linq;
using Fermion.EntityFramework.Identity.Application.DTOs.ApplicationUserSessions;
using Fermion.EntityFramework.Identity.Domain.Interfaces.Repositories;
using Fermion.EntityFramework.Identity.Domain.Interfaces.Services;
using Fermion.EntityFramework.Shared.DTOs.Pagination;
using Fermion.EntityFramework.Shared.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Fermion.EntityFramework.Identity.Application.Services;

public class ApplicationUserSessionAppService(IApplicationUserSessionRepository applicationUserSessionRepository, IMapper mapper) : IApplicationUserSessionAppService
{
    public async Task<ApplicationUserSessionResponseDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var matchedUserSession = await applicationUserSessionRepository.GetAsync(
            id: id,
            include: item =>
                item.Include(x => x.User)!,
            enableTracking: false,
            cancellationToken: cancellationToken
        );

        return mapper.Map<ApplicationUserSessionResponseDto>(matchedUserSession);
    }

    public async Task<PageableResponseDto<ApplicationUserSessionResponseDto>> GetPageableAndFilterAsync(GetListApplicationUserSessionRequestDto request, CancellationToken cancellationToken = default)
    {
        var queryable = applicationUserSessionRepository.GetQueryable();
        queryable = queryable.WhereIf(
            !string.IsNullOrWhiteSpace(request.Search),
            item =>
                item.ClientIp.Contains(request.Search!) ||
                (item.DeviceFamily != null && item.DeviceFamily.Contains(request.Search!)) ||
                (item.DeviceModel != null && item.DeviceModel.Contains(request.Search!)) ||
                (item.OsFamily != null && item.OsFamily.Contains(request.Search!)) ||
                (item.OsVersion != null && item.OsVersion.Contains(request.Search!)) ||
                (item.BrowserFamily != null && item.BrowserFamily.Contains(request.Search!)) ||
                (item.BrowserVersion != null && item.BrowserVersion.Contains(request.Search!))
        );
        queryable = queryable.WhereIf(request.UserId.HasValue, item => item.UserId == request.UserId);

        queryable = queryable.AsNoTracking();
        queryable = queryable.ApplySort(request.Field, request.Order, cancellationToken);
        var result = await queryable.ToPageableAsync(request.Page, request.PerPage, cancellationToken: cancellationToken);
        var mappedUserSessions = mapper.Map<List<ApplicationUserSessionResponseDto>>(result.Data);

        return new PageableResponseDto<ApplicationUserSessionResponseDto>(mappedUserSessions, result.Meta);
    }
}
using AutoMapper;
using Fermion.Domain.Exceptions.Types;
using Fermion.EntityFramework.Identity.Application.DTOs.ApplicationRoles;
using Fermion.EntityFramework.Identity.Application.DTOs.ApplicationUsers;
using Fermion.EntityFramework.Identity.Domain.Entities;
using Fermion.EntityFramework.Identity.Domain.Interfaces.Repositories;
using Fermion.EntityFramework.Identity.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Fermion.EntityFramework.Identity.Application.Services;

public class ApplicationUserRoleAppService(
    IApplicationUserRoleRepository applicationUserRoleRepository,
    UserManager<ApplicationUser> userManager,
    IMapper mapper) : IApplicationUserRoleAppService
{
    public async Task AssignRoleToUserAsync(Guid userId, Guid roleId, CancellationToken cancellationToken = default)
    {
        var existingUserRole = await ExistingUserRoleCheckAsync(userId, roleId, cancellationToken);
        if (existingUserRole)
        {
            throw new AppUserFriendlyException("The user already has this role assigned.");
        }

        var newUserRole = new ApplicationUserRole
        {
            UserId = userId,
            RoleId = roleId
        };

        await applicationUserRoleRepository.AddAsync(newUserRole, cancellationToken: cancellationToken);
        await applicationUserRoleRepository.SaveChangesAsync(cancellationToken);
    }

    public async Task RemoveRoleFromUserAsync(Guid userId, Guid roleId, CancellationToken cancellationToken = default)
    {
        var existingUserRole = await ExistingUserRoleCheckAsync(userId, roleId, cancellationToken);
        if (!existingUserRole)
        {
            throw new AppUserFriendlyException("The user does not have this role assigned.");
        }

        var userRole = await applicationUserRoleRepository.GetAsync(
            item =>
                item.UserId == userId &&
                item.RoleId == roleId,
            cancellationToken: cancellationToken
        );

        await applicationUserRoleRepository.DeleteAsync(userRole, cancellationToken: cancellationToken);
        await applicationUserRoleRepository.SaveChangesAsync(cancellationToken);
    }

    public async Task ClearRolesFromUserAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var matchedUserRoles = await applicationUserRoleRepository
            .GetAllAsync(item => item.UserId == userId, cancellationToken: cancellationToken);
        if (matchedUserRoles.Count == 0)
        {
            return;
        }

        await applicationUserRoleRepository.DeleteRangeAsync(matchedUserRoles, cancellationToken: cancellationToken);
        await applicationUserRoleRepository.SaveChangesAsync(cancellationToken);
    }

    public async Task<List<ApplicationRoleResponseDto>> GetRolesByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var matchedUserRoles = await applicationUserRoleRepository
            .GetAllAsync(
                item => item.UserId == userId,
                item => item
                    .Include(x => x.Role)!,
                enableTracking: false,
                cancellationToken: cancellationToken
            );

        return mapper.Map<List<ApplicationRoleResponseDto>>(matchedUserRoles.Select(item => item.Role).ToList());
    }

    public async Task<List<ApplicationUserResponseDto>> GetUsersByRoleIdAsync(Guid roleId, CancellationToken cancellationToken = default)
    {
        var matchedUserRoles = await applicationUserRoleRepository
            .GetAllAsync(
                item => item.RoleId == roleId,
                item => item
                    .Include(x => x.User)!,
                enableTracking: false,
                cancellationToken: cancellationToken
            );

        return mapper.Map<List<ApplicationUserResponseDto>>(matchedUserRoles.Select(item => item.User).ToList());
    }

    private async Task<bool> ExistingUserRoleCheckAsync(Guid userId, Guid roleId, CancellationToken cancellationToken)
    {
        return await applicationUserRoleRepository.AnyAsync(
            item =>
                item.UserId == userId &&
                item.RoleId == roleId,
            enableTracking: false,
            cancellationToken: cancellationToken
        );
    }
}
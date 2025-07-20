using AutoMapper;
using Fermion.Domain.Exceptions.Types;
using Fermion.Domain.Extensions.Linq;
using Fermion.EntityFramework.Identity.Application.DTOs.ApplicationUsers;
using Fermion.EntityFramework.Identity.Domain.Entities;
using Fermion.EntityFramework.Identity.Domain.Interfaces.Repositories;
using Fermion.EntityFramework.Identity.Domain.Interfaces.Services;
using Fermion.EntityFramework.Shared.DTOs.Pagination;
using Fermion.EntityFramework.Shared.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Fermion.EntityFramework.Identity.Application.Services;

public class ApplicationUserAppService(
    UserManager<ApplicationUser> userManager,
    IApplicationUserRepository applicationUserRepository,
    ICurrentUser currentUser,
    IMapper mapper) : IApplicationUserAppService
{
    public async Task<ApplicationUserResponseDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var matchedUser = await applicationUserRepository.GetAsync(
            item => item.Id == id,
            item => item
                .Include(x => x.UserRoles)
                .ThenInclude(x => x.Role!),
            enableTracking: false,
            cancellationToken: cancellationToken);

        var mappedUser = mapper.Map<ApplicationUserResponseDto>(matchedUser);
        mappedUser.Roles = matchedUser.UserRoles.Select(role => role.Role!.Name!).ToList();
        return mappedUser;
    }

    public async Task<PageableResponseDto<ApplicationUserResponseDto>> GetPageableAndFilterAsync(GetListApplicationUserRequestDto request, CancellationToken cancellationToken = default)
    {
        var queryable = applicationUserRepository.GetQueryable();
        queryable = queryable
            .Include(item => item.UserRoles)
            .ThenInclude(item => item.Role!);
        queryable = queryable.WhereIf(!string.IsNullOrWhiteSpace(request.Search), item =>
            item.NormalizedEmail!.Contains(request.Search!.ToUpperInvariant()) ||
            item.NormalizedUserName!.Contains(request.Search!.ToUpperInvariant())
        );

        queryable = queryable.AsNoTracking();
        queryable = queryable.ApplySort(request.Field, request.Order, cancellationToken);
        var result = await queryable.ToPageableAsync(request.Page, request.PerPage, cancellationToken: cancellationToken);
        var mappedUsers = mapper.Map<List<ApplicationUserResponseDto>>(result.Data);

        return new PageableResponseDto<ApplicationUserResponseDto>(mappedUsers, result.Meta);
    }

    public async Task<ApplicationUserResponseDto> CreateAsync(CreateApplicationUserRequestDto request, CancellationToken cancellationToken = default)
    {
        await using var transaction = await applicationUserRepository.BeginTransactionAsync();
        try
        {
            await IsUserNameExistsAsync(request.UserName, cancellationToken);
            await IsEmailExistsAsync(request.Email, cancellationToken);

            var newUser = new ApplicationUser
            {
                Id = Guid.NewGuid(),
                UserName = request.UserName,
                Email = request.Email,
                EmailConfirmed = request.EmailConfirmed,
                PhoneNumber = request.PhoneNumber,
                PhoneNumberConfirmed = request.PhoneNumberConfirmed,
                TwoFactorEnabled = request.TwoFactorEnabled,
                LockoutEnd = request.LockoutEnd,
                LockoutEnabled = request.LockoutEnabled
            };

            var result = await userManager.CreateAsync(newUser, request.Password);
            if (!result.Succeeded)
            {
                throw new AppUserFriendlyException(string.Join(", ", result.Errors.Select(e => e.Description)));
            }

            if (request.Roles.Count > 0)
            {
                var roleResult = await userManager.AddToRolesAsync(newUser, request.Roles);
                if (!roleResult.Succeeded)
                {
                    throw new AppUserFriendlyException(string.Join(", ", roleResult.Errors.Select(e => e.Description)));
                }
            }

            await transaction.CommitAsync(cancellationToken);

            return mapper.Map<ApplicationUserResponseDto>(newUser);
        }
        catch (Exception e)
        {
            await transaction.RollbackAsync(cancellationToken);
            throw new AppBusinessException("An error occurred while creating the user.", e);
        }
    }

    public async Task<ApplicationUserResponseDto> UpdateAsync(Guid id, UpdateApplicationUserRequestDto request, CancellationToken cancellationToken = default)
    {
        await using var transaction = await applicationUserRepository.BeginTransactionAsync();
        try
        {
            var matchedUser = await applicationUserRepository.GetAsync(item => item.Id == id, cancellationToken: cancellationToken);

            if (matchedUser.ConcurrencyStamp != request.ConcurrencyStamp)
            {
                throw new AppUserFriendlyException("The user has been modified by another process. Please reload and try again.");
            }

            matchedUser.PhoneNumber = request.PhoneNumber;
            matchedUser.EmailConfirmed = request.EmailConfirmed;
            matchedUser.PhoneNumberConfirmed = request.PhoneNumberConfirmed;
            matchedUser.TwoFactorEnabled = request.TwoFactorEnabled;
            matchedUser.LockoutEnd = request.LockoutEnd;
            matchedUser.LockoutEnabled = request.LockoutEnabled;

            var result = await userManager.UpdateAsync(matchedUser);
            if (!result.Succeeded)
            {
                throw new AppUserFriendlyException(string.Join(", ", result.Errors.Select(e => e.Description)));
            }

            var currentRoles = await userManager.GetRolesAsync(matchedUser);
            var rolesToAdd = request.Roles.Except(currentRoles).ToList();
            var rolesToRemove = currentRoles.Except(request.Roles).ToList();

            if (rolesToAdd.Count != 0)
            {
                var addRolesResult = await userManager.AddToRolesAsync(matchedUser, rolesToAdd);
                if (!addRolesResult.Succeeded)
                {
                    throw new AppUserFriendlyException(string.Join(", ",
                        addRolesResult.Errors.Select(e => e.Description)));
                }
            }

            if (rolesToRemove.Count != 0)
            {
                var removeRolesResult = await userManager.RemoveFromRolesAsync(matchedUser, rolesToRemove);
                if (!removeRolesResult.Succeeded)
                {
                    throw new AppUserFriendlyException(string.Join(", ", removeRolesResult.Errors.Select(e => e.Description)));
                }
            }

            await transaction.CommitAsync(cancellationToken);

            return mapper.Map<ApplicationUserResponseDto>(matchedUser);
        }
        catch (Exception e)
        {
            await transaction.RollbackAsync(cancellationToken);
            throw new AppBusinessException("An error occurred while updating the user.", e);
        }
    }

    public async Task ChangePasswordAsync(ChangePasswordApplicationUserRequestDto request, CancellationToken cancellationToken = default)
    {
        var matchedUser = await applicationUserRepository.GetAsync(item => item.Id == currentUser.Id, cancellationToken: cancellationToken);
        if (matchedUser.LockoutEnabled && matchedUser.LockoutEnd.HasValue && matchedUser.LockoutEnd.Value > DateTimeOffset.UtcNow)
        {
            var lockoutEnd = matchedUser.LockoutEnd?.ToString("yyyy-MM-dd HH:mm:ss");
            throw new AppBusinessException($"Cannot change password. Account is locked until {lockoutEnd}");
        }

        var result = await userManager.ChangePasswordAsync(matchedUser, request.Password, request.NewPassword);
        if (!result.Succeeded)
        {
            throw new AppUserFriendlyException(string.Join(", ", result.Errors.Select(e => e.Description)));
        }
    }

    public async Task<ApplicationUserResponseDto> LockAsync(Guid id, bool isPermanent, CancellationToken cancellationToken = default)
    {
        var matchedUser = await applicationUserRepository.GetAsync(item => item.Id == id, cancellationToken: cancellationToken);
        DateTimeOffset? lockoutEnd = isPermanent ? DateTimeOffset.MaxValue : DateTimeOffset.UtcNow.AddMinutes(30);

        var result = await userManager.SetLockoutEndDateAsync(matchedUser, lockoutEnd);
        if (!result.Succeeded)
        {
            throw new AppUserFriendlyException(string.Join(", ", result.Errors.Select(e => e.Description)));
        }

        return mapper.Map<ApplicationUserResponseDto>(matchedUser);
    }

    public async Task<ApplicationUserResponseDto> UnlockAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var matchedUser = await applicationUserRepository.GetAsync(item => item.Id == id, cancellationToken: cancellationToken);
        matchedUser.LockoutEnd = null;

        var result = await userManager.SetLockoutEndDateAsync(matchedUser, null);
        if (!result.Succeeded)
        {
            throw new AppUserFriendlyException(string.Join(", ", result.Errors.Select(e => e.Description)));
        }

        return mapper.Map<ApplicationUserResponseDto>(matchedUser);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var matchedUser = await applicationUserRepository.GetAsync(item => item.Id == id, cancellationToken: cancellationToken);
        await applicationUserRepository.DeleteAsync(matchedUser, cancellationToken: cancellationToken);
        await applicationUserRepository.SaveChangesAsync(cancellationToken);
    }

    private async Task IsUserNameExistsAsync(string userName, CancellationToken cancellationToken = default)
    {
        var existingUserName = await applicationUserRepository.AnyAsync(
            item => item.NormalizedUserName == userName.ToUpperInvariant(),
            enableTracking: false,
            cancellationToken: cancellationToken);

        if (existingUserName)
        {
            throw new AppUserFriendlyException($"User with UserName '{userName}' already exists.");
        }
    }

    private async Task IsEmailExistsAsync(string email, CancellationToken cancellationToken = default)
    {
        var existingEmail = await applicationUserRepository.AnyAsync(
            item => item.NormalizedEmail == email.ToUpperInvariant(),
            enableTracking: false,
            cancellationToken: cancellationToken);

        if (existingEmail)
        {
            throw new AppUserFriendlyException($"User with Email '{email}' already exists.");
        }
    }
}
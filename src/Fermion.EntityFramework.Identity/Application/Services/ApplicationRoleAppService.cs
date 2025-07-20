using AutoMapper;
using Fermion.Domain.Exceptions.Types;
using Fermion.EntityFramework.Identity.Application.DTOs.ApplicationRoles;
using Fermion.EntityFramework.Identity.Domain.Entities;
using Fermion.EntityFramework.Identity.Domain.Interfaces.Repositories;
using Fermion.EntityFramework.Identity.Domain.Interfaces.Services;
using Fermion.EntityFramework.Shared.DTOs.Pagination;
using Fermion.EntityFramework.Shared.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Fermion.EntityFramework.Identity.Application.Services;

public class ApplicationRoleAppService(IApplicationRoleRepository applicationRoleRepository, IMapper mapper) : IApplicationRoleAppService
{
    public async Task<ApplicationRoleResponseDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var matchedRole = await applicationRoleRepository.GetAsync(item => item.Id == id, cancellationToken: cancellationToken);

        return mapper.Map<ApplicationRoleResponseDto>(matchedRole);
    }

    public async Task<PageableResponseDto<ApplicationRoleResponseDto>> GetPageableAndFilterAsync(GetListApplicationRoleRequestDto request, CancellationToken cancellationToken = default)
    {
        var queryable = applicationRoleRepository.GetQueryable();

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            queryable = queryable.Where(item => item.NormalizedName!.Contains(request.Search.ToUpperInvariant()));
        }

        queryable = queryable.AsNoTracking();
        queryable = queryable.ApplySort(request.Field, request.Order, cancellationToken);
        var result = await queryable.ToPageableAsync(request.Page, request.PerPage, cancellationToken: cancellationToken);
        var mappedRoles = mapper.Map<List<ApplicationRoleResponseDto>>(result.Data);

        return new PageableResponseDto<ApplicationRoleResponseDto>(mappedRoles, result.Meta);
    }

    public async Task<List<ApplicationRoleResponseDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var matchedRoles = await applicationRoleRepository.GetAllAsync(cancellationToken: cancellationToken);

        return mapper.Map<List<ApplicationRoleResponseDto>>(matchedRoles);
    }

    public async Task<ApplicationRoleResponseDto> CreateAsync(CreateApplicationRoleRequestDto request, CancellationToken cancellationToken = default)
    {
        var existingRole = await applicationRoleRepository.AnyAsync(
            item => 
                item.NormalizedName == request.Name.ToUpperInvariant(),
            enableTracking: false,
            cancellationToken: cancellationToken);
        
        if (existingRole)
        {
            throw new AppUserFriendlyException($"Role with name '{request.Name}' already exists.");
        }

        var newRole = new ApplicationRole(request.Name, request.Description);
        newRole.NormalizedName = request.Name.ToUpperInvariant();
        newRole = await applicationRoleRepository.AddAsync(newRole, cancellationToken: cancellationToken);
        await applicationRoleRepository.SaveChangesAsync(cancellationToken: cancellationToken);

        return mapper.Map<ApplicationRoleResponseDto>(newRole);
    }

    public async Task<ApplicationRoleResponseDto> UpdateAsync(Guid id, UpdateApplicationRoleRequestDto request, CancellationToken cancellationToken = default)
    {
        var matchedRole =  await applicationRoleRepository.GetAsync(item => item.Id == id, cancellationToken: cancellationToken);
        
        var existingRole = await applicationRoleRepository.AnyAsync(
            item => 
                item.NormalizedName == request.Name.ToUpperInvariant(),
            enableTracking: false,
            cancellationToken: cancellationToken);
        
        if (matchedRole.Name != request.Name && existingRole)
        {
            throw new AppUserFriendlyException($"Role with name '{request.Name}' already exists.");
        }

        if (matchedRole.ConcurrencyStamp != request.ConcurrencyStamp)
        {
            throw new AppUserFriendlyException("The role has been modified by another user. Please reload and try again.");
        }

        matchedRole.Name = request.Name;
        matchedRole.Description = request.Description;
        matchedRole.NormalizedName = request.Name.ToUpperInvariant();
        matchedRole = await applicationRoleRepository.UpdateAsync(matchedRole, cancellationToken: cancellationToken);
        await applicationRoleRepository.SaveChangesAsync(cancellationToken: cancellationToken);

        return mapper.Map<ApplicationRoleResponseDto>(matchedRole);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var matchedRole =  await applicationRoleRepository.GetAsync(item => item.Id == id, cancellationToken: cancellationToken);
        await applicationRoleRepository.DeleteAsync(matchedRole, cancellationToken: cancellationToken);
        await applicationRoleRepository.SaveChangesAsync(cancellationToken: cancellationToken);
    }
}
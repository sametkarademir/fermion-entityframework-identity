using AutoMapper;
using Fermion.EntityFramework.Identity.Application.DTOs.ApplicationRoles;
using Fermion.EntityFramework.Identity.Application.DTOs.ApplicationUsers;
using Fermion.EntityFramework.Identity.Application.DTOs.ApplicationUserSessions;
using Fermion.EntityFramework.Identity.Domain.Entities;

namespace Fermion.EntityFramework.Identity.Application.Profiles;

public class EntityProfiles : Profile
{
    public EntityProfiles()
    {
        CreateMap<ApplicationRole, ApplicationRoleResponseDto>();
        CreateMap<ApplicationUser, ApplicationUserResponseDto>();
        CreateMap<ApplicationUserSession, ApplicationUserSessionResponseDto>();
    }
}
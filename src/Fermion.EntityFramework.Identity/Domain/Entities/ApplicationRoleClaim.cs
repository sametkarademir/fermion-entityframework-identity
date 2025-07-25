using Fermion.Domain.Shared.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace Fermion.EntityFramework.Identity.Domain.Entities;

public class ApplicationRoleClaim : IdentityRoleClaim<Guid>, ICreationAuditedObject, IEntity
{
    public DateTime CreationTime { get; set; }
    public Guid? CreatorId { get; set; }

    public ApplicationRole? Role { get; set; }
}
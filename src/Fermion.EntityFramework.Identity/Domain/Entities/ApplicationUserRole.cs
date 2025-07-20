using Fermion.Domain.Shared.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace Fermion.EntityFramework.Identity.Domain.Entities;

public class ApplicationUserRole : IdentityUserRole<Guid>, IFullAuditedObject, IEntity
{
    public DateTime CreationTime { get; set; }
    public Guid? CreatorId { get; set; }
    public DateTime? LastModificationTime { get; set; }
    public Guid? LastModifierId { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? DeletionTime { get; set; }
    public Guid? DeleterId { get; set; }
    
    public ApplicationUser? User { get; set; }
    public ApplicationRole? Role { get; set; }

    public ApplicationUserRole()
    {
        
    }

    public ApplicationUserRole(Guid userId, Guid roleId) 
    {
        UserId = userId;
        RoleId = roleId;
    }
}
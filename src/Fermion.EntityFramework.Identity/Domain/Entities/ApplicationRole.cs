using Fermion.Domain.Shared.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace Fermion.EntityFramework.Identity.Domain.Entities;

public class ApplicationRole : IdentityRole<Guid>, IFullAuditedObject, IEntity
{
    public string? Description { get; set; }

    public DateTime CreationTime { get; set; }
    public Guid? CreatorId { get; set; }
    public DateTime? LastModificationTime { get; set; }
    public Guid? LastModifierId { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? DeletionTime { get; set; }
    public Guid? DeleterId { get; set; }

    public ICollection<ApplicationUserRole> UserRoles { get; set; } = [];

    public ApplicationRole()
    {
        ConcurrencyStamp = Guid.NewGuid().ToString("N");
    }
    
    public ApplicationRole(string roleName) : base(roleName)
    {
        ConcurrencyStamp = Guid.NewGuid().ToString("N");
    }
    
    public ApplicationRole(string roleName, string? description) : base(roleName)
    {
        ConcurrencyStamp = Guid.NewGuid().ToString("N");
        Description = description;
    }
}
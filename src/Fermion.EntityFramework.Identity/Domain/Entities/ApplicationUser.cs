using Fermion.Domain.Shared.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace Fermion.EntityFramework.Identity.Domain.Entities;

public class ApplicationUser : IdentityUser<Guid>, IFullAuditedObject, IEntity
{
    public DateTime? PasswordLastSetTime { get; set; }
    
    public DateTime CreationTime { get; set; }
    public Guid? CreatorId { get; set; }
    public DateTime? LastModificationTime { get; set; }
    public Guid? LastModifierId { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? DeletionTime { get; set; }
    public Guid? DeleterId { get; set; }
    
    public ICollection<ApplicationUserRole> UserRoles { get; set; } = [];
    public ICollection<ApplicationUserSession> UserSessions { get; set; } = [];

    public ApplicationUser()
    {
        ConcurrencyStamp = Guid.NewGuid().ToString("N");
    }
}
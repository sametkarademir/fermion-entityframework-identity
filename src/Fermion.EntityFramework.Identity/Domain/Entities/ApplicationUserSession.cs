using Fermion.Domain.Shared.Auditing;
using Fermion.Domain.Shared.Interfaces;

namespace Fermion.EntityFramework.Identity.Domain.Entities;

public class ApplicationUserSession : FullAuditedEntity<Guid>, IEntityCorrelationId, IEntitySnapshotId
{
    public string ClientIp { get; set; } = null!;
    public string UserAgent { get; set; } = null!;

    public string? DeviceFamily { get; set; }
    public string? DeviceModel { get; set; }
    public string? OsFamily { get; set; }
    public string? OsVersion { get; set; }
    public string? BrowserFamily { get; set; }
    public string? BrowserVersion { get; set; }
    
    public bool IsMobile { get; set; }
    public bool IsDesktop { get; set; }
    public bool IsTablet { get; set; }

    public Guid? CorrelationId { get; set; }
    public Guid? SnapshotId { get; set; }

    public Guid UserId { get; set; }
    public ApplicationUser? User { get; set; }

    public ApplicationUserSession()
    {
        
    }

    public ApplicationUserSession(Guid id): base(id)
    {
        
    }
}
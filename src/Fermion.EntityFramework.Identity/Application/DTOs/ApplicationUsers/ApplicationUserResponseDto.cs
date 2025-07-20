using Fermion.Domain.Shared.DTOs;

namespace Fermion.EntityFramework.Identity.Application.DTOs.ApplicationUsers;

public class ApplicationUserResponseDto : FullAuditedEntityDto<Guid>
{
    public string UserName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string? PhoneNumber { get; set; }
    
    public bool EmailConfirmed { get; set; }
    public bool PhoneNumberConfirmed { get; set; }
    public bool TwoFactorEnabled { get; set; }
    
    public DateTime? PasswordChangedTime { get; set; }
    public DateTimeOffset? LockoutEnd { get; set; }
    public bool LockoutEnabled { get; set; }
    public int AccessFailedCount { get; set; }
    public bool IsLocked => LockoutEnd != null && LockoutEnd > DateTimeOffset.Now;
    public string ConcurrencyStamp { get; set; } = null!;

    public List<string> Roles { get; set; } = [];
}
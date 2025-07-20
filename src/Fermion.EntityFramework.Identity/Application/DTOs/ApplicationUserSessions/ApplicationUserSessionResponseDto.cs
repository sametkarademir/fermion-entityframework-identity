using Fermion.Domain.Shared.DTOs;
using Fermion.EntityFramework.Identity.Application.DTOs.ApplicationUsers;

namespace Fermion.EntityFramework.Identity.Application.DTOs.ApplicationUserSessions;

public class ApplicationUserSessionResponseDto : FullAuditedEntityDto<Guid>
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

    public Guid UserId { get; set; }
    public ApplicationUserResponseDto? User { get; set; }
}
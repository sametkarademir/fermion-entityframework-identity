using Fermion.EntityFramework.Identity.Domain.Entities;
using Fermion.EntityFramework.Identity.Infrastructure.EntityConfigurations;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Fermion.EntityFramework.Identity.Infrastructure.Contexts;

public abstract class IdentityUserDbContext 
    : IdentityDbContext<
        ApplicationUser, 
        ApplicationRole, 
        Guid, 
        ApplicationUserClaim, 
        ApplicationUserRole, 
        ApplicationUserLogin, 
        ApplicationRoleClaim, 
        ApplicationUserToken>
{
    public DbSet<ApplicationUserSession> UserSessions { get; set; }
 
    public IdentityUserDbContext(DbContextOptions options) : base(options)
    {
    }
    
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(typeof(ApplicationRoleClaimConfiguration).Assembly);
        builder.ApplyConfigurationsFromAssembly(typeof(ApplicationRoleConfiguration).Assembly);
        builder.ApplyConfigurationsFromAssembly(typeof(ApplicationUserClaimConfiguration).Assembly);
        builder.ApplyConfigurationsFromAssembly(typeof(ApplicationUserLoginConfiguration).Assembly);
        builder.ApplyConfigurationsFromAssembly(typeof(ApplicationUserConfiguration).Assembly);
        builder.ApplyConfigurationsFromAssembly(typeof(ApplicationUserRoleConfiguration).Assembly);
        builder.ApplyConfigurationsFromAssembly(typeof(ApplicationUserTokenConfiguration).Assembly);
        builder.ApplyConfigurationsFromAssembly(typeof(ApplicationUserSessionConfiguration).Assembly);
    }
}
using Fermion.EntityFramework.Identity.Domain.Entities;
using Fermion.EntityFramework.Shared.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Fermion.EntityFramework.Identity.Infrastructure.EntityConfigurations;

public class ApplicationUserRoleConfiguration : IEntityTypeConfiguration<ApplicationUserRole>
{
    public void Configure(EntityTypeBuilder<ApplicationUserRole> builder)
    {
        builder.ApplyGlobalEntityConfigurations();

        // Primary key
        builder.HasKey(r => new { r.UserId, r.RoleId });
        builder.HasIndex(r => new { r.UserId, r.RoleId }).IsUnique();

        // Maps to the AspNetUserRoles table
        builder.ToTable("ApplicationUserRoles");

        // Relationships
        builder.HasOne<ApplicationUser>(item => item.User)
            .WithMany(item => item.UserRoles)
            .HasForeignKey(item => item.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<ApplicationRole>(item => item.Role)
            .WithMany(item => item.UserRoles)
            .HasForeignKey(item => item.RoleId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
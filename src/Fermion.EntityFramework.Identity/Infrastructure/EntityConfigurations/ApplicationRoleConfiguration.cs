using Fermion.EntityFramework.Identity.Domain.Entities;
using Fermion.EntityFramework.Shared.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Fermion.EntityFramework.Identity.Infrastructure.EntityConfigurations;

public class ApplicationRoleConfiguration : IEntityTypeConfiguration<ApplicationRole>
{
    public void Configure(EntityTypeBuilder<ApplicationRole> builder)
    {
        builder.ApplyGlobalEntityConfigurations();
        
        // Primary key
        builder.HasKey(r => r.Id);
        builder.HasIndex(r => r.NormalizedName).HasDatabaseName("RoleNameIndex").IsUnique(false);
        builder.HasIndex(item => new { item.NormalizedName, item.DeletionTime }).IsUnique(); 

        // Maps to the AspNetRoles table
        builder.ToTable("ApplicationRoles");

        // A concurrency token for use with the optimistic concurrency checking
        builder.Property(r => r.ConcurrencyStamp).IsConcurrencyToken();

        // Limit the size of columns to use efficient database types
        builder.Property(r => r.Name).HasMaxLength(256).IsRequired();
        builder.Property(r => r.NormalizedName).HasMaxLength(256).IsRequired();
        builder.Property(r => r.Description).HasMaxLength(1024).IsRequired(false);

        // Each Role can have many RoleClaims
        builder.HasMany<ApplicationRoleClaim>().WithOne(rc => rc.Role).HasForeignKey(rc => rc.RoleId).IsRequired();
        builder.HasMany<ApplicationUserRole>().WithOne(rc => rc.Role).HasForeignKey(rc => rc.RoleId).IsRequired();
    }
}
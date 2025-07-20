using Fermion.EntityFramework.Identity.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Fermion.EntityFramework.Identity.Infrastructure.EntityConfigurations;

public class ApplicationUserLoginConfiguration : IEntityTypeConfiguration<ApplicationUserLogin>
{
    public void Configure(EntityTypeBuilder<ApplicationUserLogin> builder)
    {
        // Primary key
        builder.HasKey(ul => new { ul.LoginProvider, ul.ProviderKey });

        // Maps to the AspNetUserLogins table
        builder.ToTable("ApplicationUserLogins");

        // Limit the size of the composite key columns due to common DB restrictions
        builder.Property(l => l.LoginProvider).HasMaxLength(128);
        builder.Property(l => l.ProviderKey).HasMaxLength(128);
    }
}
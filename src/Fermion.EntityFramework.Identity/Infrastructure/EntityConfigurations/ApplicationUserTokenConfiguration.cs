using Fermion.EntityFramework.Identity.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Fermion.EntityFramework.Identity.Infrastructure.EntityConfigurations;

public class ApplicationUserTokenConfiguration : IEntityTypeConfiguration<ApplicationUserToken>
{
    public void Configure(EntityTypeBuilder<ApplicationUserToken> builder)
    {
        // Composite primary key consisting of the UserId, LoginProvider and Name
        builder.HasKey(t => new { t.UserId, t.LoginProvider, t.Name });

        // Maps to the AspNetUserTokens table
        builder.ToTable("ApplicationUserTokens");

        // Limit the size of the composite key columns due to common DB restrictions
        builder.Property(t => t.LoginProvider).HasMaxLength(256);
        builder.Property(t => t.Name).HasMaxLength(256);
    }
}
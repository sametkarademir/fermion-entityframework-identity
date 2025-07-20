using Fermion.EntityFramework.Identity.Domain.Entities;
using Fermion.EntityFramework.Shared.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Fermion.EntityFramework.Identity.Infrastructure.EntityConfigurations;

public class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        builder.ApplyGlobalEntityConfigurations();

        // Primary key
        builder.HasKey(u => u.Id);

        // Indexes for "normalized" username and email, to allow efficient lookups
        builder.HasIndex(u => u.NormalizedUserName).HasDatabaseName("UserNameIndex").IsUnique(false);
        builder.HasIndex(u => u.NormalizedEmail).HasDatabaseName("EmailIndex").IsUnique(false);
        builder.HasIndex(u => new { u.NormalizedUserName, u.DeletionTime }).IsUnique();
        builder.HasIndex(u => new { u.NormalizedEmail, u.DeletionTime }).IsUnique();

        // Maps to the AspNetUsers table
        builder.ToTable("ApplicationUsers");

        // A concurrency token for use with the optimistic concurrency checking
        builder.Property(u => u.ConcurrencyStamp).IsConcurrencyToken();

        // Limit the size of columns to use efficient database types
        builder.Property(u => u.UserName).HasMaxLength(16).IsRequired();
        builder.Property(u => u.NormalizedUserName).HasMaxLength(16).IsRequired();
        builder.Property(u => u.Email).HasMaxLength(256).IsRequired();
        builder.Property(u => u.NormalizedEmail).HasMaxLength(256).IsRequired();

        // The relationships between User and other entity types
        // Note that these relationships are configured with no navigation properties

        // Each User can have many UserClaims
        builder.HasMany<ApplicationUserClaim>().WithOne(uc => uc.User).HasForeignKey(uc => uc.UserId).IsRequired();

        // Each User can have many UserLogins
        builder.HasMany<ApplicationUserLogin>().WithOne(ul => ul.User).HasForeignKey(ul => ul.UserId).IsRequired();

        // Each User can have many UserTokens
        builder.HasMany<ApplicationUserToken>().WithOne(ut => ut.User).HasForeignKey(ut => ut.UserId).IsRequired();

        // Each User can have many entries in the UserRole join table
        builder.HasMany<ApplicationUserRole>().WithOne(ur => ur.User).HasForeignKey(ur => ur.UserId).IsRequired();
        builder.HasMany<ApplicationUserSession>().WithOne(us => us.User).HasForeignKey(us => us.UserId).IsRequired();
    }
}
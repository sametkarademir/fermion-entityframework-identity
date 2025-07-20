using Fermion.EntityFramework.Identity.Domain.Entities;
using Fermion.EntityFramework.Shared.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Fermion.EntityFramework.Identity.Infrastructure.EntityConfigurations;

public class ApplicationUserSessionConfiguration : IEntityTypeConfiguration<ApplicationUserSession>
{
    public void Configure(EntityTypeBuilder<ApplicationUserSession> builder)
    {
        builder.ApplyGlobalEntityConfigurations();

        builder.ToTable("ApplicationUserSessions");
        builder.HasIndex(item => item.IsMobile);
        builder.HasIndex(item => item.IsDesktop);
        builder.HasIndex(item => item.IsTablet);
        builder.HasIndex(item => item.ClientIp);

        builder.Property(item => item.ClientIp).HasMaxLength(50).IsRequired();
        builder.Property(item => item.UserAgent).HasMaxLength(500).IsRequired();
        builder.Property(item => item.DeviceFamily).HasMaxLength(256).IsRequired(false);
        builder.Property(item => item.DeviceModel).HasMaxLength(256).IsRequired(false);
        builder.Property(item => item.OsFamily).HasMaxLength(256).IsRequired(false);
        builder.Property(item => item.OsVersion).HasMaxLength(256).IsRequired(false);
        builder.Property(item => item.BrowserFamily).HasMaxLength(256).IsRequired(false);
        builder.Property(item => item.BrowserVersion).HasMaxLength(256).IsRequired(false);
        builder.Property(item => item.IsMobile).IsRequired();
        builder.Property(item => item.IsDesktop).IsRequired();
        builder.Property(item => item.IsTablet).IsRequired();
        
        builder.HasOne(item => item.User)
            .WithMany()
            .HasForeignKey(item => item.UserId)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired();
    }
}
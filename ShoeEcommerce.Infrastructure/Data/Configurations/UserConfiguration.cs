using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShoeEcommerce.Domain.Entities;

namespace ShoeEcommerce.Infrastructure.Data.Configurations;
public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");

        builder.HasKey(u => u.Id);


        builder.Property(u => u.Email)
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(u => u.NormalizedEmail)
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(u => u.FullName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(u => u.Username)
            .HasMaxLength(100);

        builder.Property(u => u.NormalizedUsername)
            .HasMaxLength(100);

        builder.Property(u => u.PhoneNumber)
            .HasMaxLength(32);

        builder.Property(u => u.NormalizedPhoneNumber)
            .HasMaxLength(32);

        builder.Property(u => u.PasswordHash)
            .IsRequired()
            .HasMaxLength(512);

        builder.Property(u => u.SecurityStamp)
            .HasMaxLength(200);

        builder.HasIndex(u => u.NormalizedEmail)
            .IsUnique()
            .HasDatabaseName("IX_Users_NormalizedEmail");

        builder.HasIndex(u => u.NormalizedUsername)
            .IsUnique()
            .HasDatabaseName("IX_Users_NormalizedUsername")
            .HasFilter("[NormalizedUsername] IS NOT NULL");

        builder.HasIndex(u => u.NormalizedPhoneNumber)
            .IsUnique()
            .HasDatabaseName("IX_Users_NormalizedPhone")
            .HasFilter("[NormalizedPhoneNumber] IS NOT NULL");

        builder.HasIndex(u => u.CreatedAt)
            .HasDatabaseName("IX_Users_CreatedAt");

        builder.HasOne(u => u.Role)
            .WithMany(r => r.Users)
            .HasForeignKey(u => u.RoleId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(u => u.RefreshTokens)
            .WithOne(rt => rt.User)
            .HasForeignKey(rt => rt.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(u => u.UserBlocks)
            .WithOne(ub => ub.User)
            .HasForeignKey(ub => ub.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(u => u.EmailConfirmed).HasDefaultValue(false);
        builder.Property(u => u.PhoneNumberConfirmed).HasDefaultValue(false);
        builder.Property(u => u.IsDeleted).HasDefaultValue(false);
        builder.Property(u => u.AccessFailedCount).HasDefaultValue(0);
        builder.Property(u => u.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
    }
}
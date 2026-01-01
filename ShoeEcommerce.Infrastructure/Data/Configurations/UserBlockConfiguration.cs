using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShoeEcommerce.Domain.Entities;

public class UserBlockConfiguration : IEntityTypeConfiguration<UserBlock>
{
    public void Configure(EntityTypeBuilder<UserBlock> builder)
    {
        builder.ToTable("UserBlocks");
        builder.HasKey(ub => ub.Id);

        builder.Property(ub => ub.Reason).HasMaxLength(500);
        builder.Property(ub => ub.RevokedReason).HasMaxLength(500);

        builder.HasOne(ub => ub.User)
            .WithMany(u => u.UserBlocks)
            .HasForeignKey(ub => ub.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(ub => ub.Admin)
            .WithMany()
            .HasForeignKey(ub => ub.AdminId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(ub => ub.RevokedByAdmin)
            .WithMany()
            .HasForeignKey(ub => ub.RevokedByAdminId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Ignore(ub => ub.IsActive);

        builder.Property(ub => ub.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
    }
}
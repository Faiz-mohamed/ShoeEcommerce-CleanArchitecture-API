using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShoeEcommerce.Domain.Entities;

public class AuditLogConfiguration : IEntityTypeConfiguration<AuditLog>
{
    public void Configure(EntityTypeBuilder<AuditLog> builder)
    {
        builder.ToTable("AuditLogs");
        builder.HasKey(al => al.Id);

        builder.Property(al => al.Action)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(al => al.Details)
            .HasColumnType("nvarchar(max)");

        builder.HasIndex(al => al.ActorId)
            .HasDatabaseName("IX_AuditLogs_ActorId");

        builder.HasIndex(al => al.TargetUserId)
            .HasDatabaseName("IX_AuditLogs_TargetUserId");

        builder.HasIndex(al => al.CreatedAt)
            .HasDatabaseName("IX_AuditLogs_CreatedAt");

        builder.HasOne(al => al.Actor)
            .WithMany()
            .HasForeignKey(al => al.ActorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(al => al.TargetUser)
            .WithMany()
            .HasForeignKey(al => al.TargetUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(al => al.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
    }
}
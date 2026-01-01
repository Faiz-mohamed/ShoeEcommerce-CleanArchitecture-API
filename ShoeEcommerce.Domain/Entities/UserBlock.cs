using ShoeEcommerce.Domain.Common;

namespace ShoeEcommerce.Domain.Entities;
public class UserBlock : BaseEntity
{
    public Guid UserId { get; set; }
    public Guid AdminId { get; set; }
    public string? Reason { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public DateTime? RevokedAt { get; set; }
    public Guid? RevokedByAdminId { get; set; }
    public string? RevokedReason { get; set; }
    public bool IsActive =>
        RevokedAt == null &&
        (ExpiresAt == null || DateTime.UtcNow < ExpiresAt);

    public User User { get; set; } = null!;

    /// <summary>
    /// The admin who created the block
    /// 
    /// USAGE:
    /// Console.WriteLine($"Blocked by: {block.Admin.FullName}");
    /// 
    /// EF CORE CONFIGURATION NEEDED:
    /// Since both User and Admin point to Users table,
    /// we need to tell EF Core which is which in configuration
    /// (We'll do this in Infrastructure layer)
    /// </summary>
    public User Admin { get; set; } = null!;
    public User? RevokedByAdmin { get; set; }
}
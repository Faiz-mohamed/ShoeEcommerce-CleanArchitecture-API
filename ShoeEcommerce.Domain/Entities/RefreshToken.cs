using ShoeEcommerce.Domain.Common;

namespace ShoeEcommerce.Domain.Entities;
public class RefreshToken : BaseEntity
{
    public Guid UserId { get; set; }
    public string Token { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public string? CreatedByIp { get; set; }
    public DateTime? RevokedAt { get; set; }
    public string? RevokedByIp { get; set; }
    public string? ReplacementToken { get; set; }
    /// EXAMPLES:
    /// - "User logged out"
    /// - "User logged out from all devices"
    /// - "Password changed"
    /// - "Email changed"
    /// - "Account locked by admin"
    /// - "Suspicious activity detected"
    /// - "Token rotation - replaced with new token"
    /// - "Security stamp mismatch"
    /// - "User requested account deletion"
 
    public string? ReasonRevoked { get; set; }
    public string? SecurityStamp { get; set; }
    public bool IsActive => RevokedAt == null && DateTime.UtcNow < ExpiresAt;
    public bool IsExpired => DateTime.UtcNow >= ExpiresAt;
    public User User { get; set; } = null!;
}
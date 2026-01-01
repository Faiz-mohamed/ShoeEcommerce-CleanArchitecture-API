using ShoeEcommerce.Domain.Common;
using System.Data;

namespace ShoeEcommerce.Domain.Entities;
public class User : BaseEntity
{
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string NormalizedEmail { get; set; } = string.Empty;
    public bool EmailConfirmed { get; set; } = false;
    public string? Username { get; set; }
    public string? NormalizedUsername { get; set; }
    public string? PhoneNumber { get; set; }
    public string? NormalizedPhoneNumber { get; set; }
    public bool PhoneNumberConfirmed { get; set; }
    public string PasswordHash { get; set; } = string.Empty;
    public string? SecurityStamp { get; set; }
    public DateTime? LockoutEndAt { get; set; }
    public int AccessFailedCount { get; set; } = 0;
    public bool IsDeleted { get; set; } = false;
    public Guid RoleId { get; set; }

    public Role Role { get; set; } = null!;
    public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
    public ICollection<UserBlock> UserBlocks { get; set; } = new List<UserBlock>();
}
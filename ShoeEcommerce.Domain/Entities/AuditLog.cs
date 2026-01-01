using ShoeEcommerce.Domain.Common;

namespace ShoeEcommerce.Domain.Entities;
public class AuditLog : BaseEntity
{
    public Guid? ActorId { get; set; }
    public string Action { get; set; } = string.Empty;
    public Guid? TargetUserId { get; set; }
    public string? Details { get; set; }
    public User? Actor { get; set; }
    public User? TargetUser { get; set; }
}
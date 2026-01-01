using ShoeEcommerce.Domain.Common;

namespace ShoeEcommerce.Domain.Entities;
public class Role : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsActive { get; set; } = true;
    public ICollection<User> Users { get; set; } = new List<User>();
}
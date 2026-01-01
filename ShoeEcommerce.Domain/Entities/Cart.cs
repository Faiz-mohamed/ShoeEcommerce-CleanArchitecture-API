using ShoeEcommerce.Domain.Common;

namespace ShoeEcommerce.Domain.Entities
{
    public class Cart : BaseEntity
    {
        public Guid UserId { get; set; }
        public virtual User User { get; set; } = null!; 
        public virtual ICollection<CartItem> Items { get; set; } = new List<CartItem>();
        public decimal TotalPrice => Items.Sum(i => i.Quantity * i.UnitPrice);
    }
}
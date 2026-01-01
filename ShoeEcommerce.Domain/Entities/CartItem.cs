using ShoeEcommerce.Domain.Common;

namespace ShoeEcommerce.Domain.Entities
{
    public class CartItem : BaseEntity
    {
        public Guid CartId { get; set; }
        public virtual Cart Cart { get; set; } = null!;
        public Guid ProductVariantId { get; set; }
        public virtual ProductVariants ProductVariant { get; set; } = null!;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
    }
}
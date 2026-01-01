using ShoeEcommerce.Domain.Common;

namespace ShoeEcommerce.Domain.Entities
{
    public class ProductVariants : BaseEntity
    {
        public Guid ProductId { get; set; }
        public virtual Product Product { get; set; } = null!;
        public string Sku { get; set; } = null!;
        public string? Size { get; set; }
        public string? Colour { get; set; }
        public required decimal Price { get; set; }
        public decimal? Weight { get; set; }
        public int InventoryQty { get; set; }
        public bool IsActive { get; set; } = true;
        public bool IsDeleted { get; set; } = false;
        public virtual ICollection<ProductImages> ProductImages { get; set; } = new List<ProductImages>();
    }
}
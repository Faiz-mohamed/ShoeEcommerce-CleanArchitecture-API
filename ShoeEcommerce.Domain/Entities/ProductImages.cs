using ShoeEcommerce.Domain.Common;

namespace ShoeEcommerce.Domain.Entities
{
    public class ProductImages : BaseEntity
    {
        public Guid ProductId { get; set; }
        public virtual Product Product { get; set; } = null!;

        public Guid VariantId { get; set; }
        public virtual ProductVariants Variant { get; set; } = null!;

        public string? ImagesJson { get; set; }
        public bool IsDeleted { get; set; } = false;
    }
}
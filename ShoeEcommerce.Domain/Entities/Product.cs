using ShoeEcommerce.Domain.Common;

namespace ShoeEcommerce.Domain.Entities
{
    public class Product : BaseEntity
    {
        public string Name { get; set; } = null!;
        public string Slug { get; set; } = null!;
        public string? Description { get; set; }
        public bool Status { get; set; } = true;
        public bool IsDeleted { get; set; } = false;
        public string MainImageUrl { get; set; } = null!;
        public Guid? BrandId { get; set; }
        public virtual Brand? Brand { get; set; }
        public virtual ICollection<ProductVariants> Variants { get; set; } = new List<ProductVariants>();
        public virtual ICollection<ProductImages> Images { get; set; } = new List<ProductImages>();
        public virtual ICollection<ProductCategory> ProductCategories { get; set; } = new List<ProductCategory>();
    }
}
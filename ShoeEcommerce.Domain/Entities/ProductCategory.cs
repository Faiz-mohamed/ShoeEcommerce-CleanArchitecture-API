namespace ShoeEcommerce.Domain.Entities
{
    public class ProductCategory 
    {
        public Guid ProductId { get; set; }
        public Guid CategoryId { get; set; }
        public virtual Product Product { get; set; } = null!;
        public virtual Category Category { get; set; } = null!;
    }
}
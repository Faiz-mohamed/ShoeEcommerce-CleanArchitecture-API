using ShoeEcommerce.Domain.Common;

namespace ShoeEcommerce.Domain.Entities
{
    public class WishlistItem : BaseEntity
    {
        public Guid WishlistId { get; set; }
        public virtual Wishlist Wishlist { get; set; } = null!;
        public Guid ProductId { get; set; }
        public virtual Product Product { get; set; } = null!;
    }
}
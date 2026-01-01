using ShoeEcommerce.Domain.Common;

namespace ShoeEcommerce.Domain.Entities
{
    public class Wishlist : BaseEntity
    {
        public Guid UserId { get; set; }
        public virtual ICollection<WishlistItem> Items { get; set; } = new List<WishlistItem>();
    }
}
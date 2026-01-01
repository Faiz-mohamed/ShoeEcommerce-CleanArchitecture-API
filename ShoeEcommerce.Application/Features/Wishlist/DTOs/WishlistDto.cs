namespace ShoeEcommerce.Application.Features.Wishlist.DTOs
{
    public class WishlistDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public List<WishlistItemDto> Items { get; set; } = new();
    }

    public class WishlistItemDto
    {
        public Guid ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string BrandName { get; set; } = string.Empty;
        public string MainImageUrl { get; set; } = string.Empty;
    }
}
namespace ShoeEcommerce.Application.Features.Cart.DTOs
{
    public class AddToCartRequest
    {
        public Guid ProductVariantId { get; set; }
        public int Quantity { get; set; }
    }
}
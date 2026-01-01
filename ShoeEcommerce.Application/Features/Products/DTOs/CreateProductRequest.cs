namespace ShoeEcommerce.Application.Features.Products.DTOs
{
    public class CreateProductRequest
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string MainImageUrl { get; set; } = string.Empty;
        public bool Status { get; set; } = true;
        public Guid? BrandId { get; set; }
        public List<Guid> CategoryIds { get; set; } = new();
        public List<CreateProductVariantDto> Variants { get; set; } = new();
    }

    public class CreateProductVariantDto
    {
        public string Sku { get; set; } = string.Empty;
        public string? Size { get; set; }
        public string? Colour { get; set; }
        public decimal Price { get; set; }
        public decimal? Weight { get; set; }
        public int InventoryQty { get; set; }
        public bool IsActive { get; set; } = true;
        public List<string> ImageUrls { get; set; } = new();
    }
}
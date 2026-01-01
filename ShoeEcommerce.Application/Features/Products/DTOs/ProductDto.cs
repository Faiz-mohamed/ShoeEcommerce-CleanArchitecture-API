namespace ShoeEcommerce.Application.Features.Products.DTOs
{
    public class ProductDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public string BrandName { get; set; } = string.Empty;
        public string MainImageUrl { get; set; } = string.Empty;
        public decimal PriceStart { get; set; }
        public string? CategoryName {  get; set; } = string.Empty;
    }

    public class ProductDetailDto : ProductDto
    {
        public string? Description { get; set; }
        public List<ProductVariantDto> Variants { get; set; } = new();
    }

    public class ProductVariantDto
    {
        public Guid Id { get; set; }
        public string Sku { get; set; } = string.Empty;
        public string Size { get; set; } = string.Empty;
        public string Color { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public List<string> Images { get; set; } = new();
    }

    public class PagedResult<T>
    {
        public List<T> Items { get; set; } = new();
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }
}
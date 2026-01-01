namespace ShoeEcommerce.Application.Features.Product.Queries.GetProductsPaged
{
    public class GetProductsPagedQuery
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public Guid? CategoryId { get; set; }
        public string? CategorySlug { get; set; } = null;
        public string? SearchTerm { get; set; } = null;
    }
}
using ShoeEcommerce.Domain.Entities;

namespace ShoeEcommerce.Application.Interfaces.Repositories
{
    public interface IProductRepository
    {
        Task<Product?> GetByIdAsync(Guid productId);
        Task<Product?> GetByVariantIdAsync(Guid variantId);
        Task<(List<Product> Items, int TotalCount)> GetPagedAsync(
            int pageNumber,
            int pageSize,
            Guid? categoryId = null,
            string? categorySlug = null,
            string? searchTerm = null
        );
        Task<bool> IsNameUniqueAsync(string name);
        Task AddAsync(Product product);
    }
}
using ShoeEcommerce.Application.Features.Products.DTOs;

namespace ShoeEcommerce.Application.Common.Interfaces.Services
{
    public interface IProductService
    {
        Task<ProductDetailDto?> GetProductByIdAsync(Guid id);
        Task<PagedResult<ProductDto>> GetProductsPagedAsync(int pageNumber, int pageSize, Guid? categoryId = null);
        Task<List<ProductDto>> GetProductsByCategoryAsync(string categorySlug);
    }
}

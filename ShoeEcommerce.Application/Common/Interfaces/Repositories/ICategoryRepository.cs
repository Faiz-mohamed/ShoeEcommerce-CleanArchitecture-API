using ShoeEcommerce.Domain.Entities;

namespace ShoeEcommerce.Application.Common.Interfaces.Repositories
{
    public interface ICategoryRepository
    {
        Task<Category?> GetByIdAsync(Guid id);
        Task<Category?> GetBySlugAsync(string slug);
        Task<bool> IsNameUniqueAsync(string name);
        Task<bool> ExistsAsync(Guid id);
        Task AddAsync(Category category);
    }
}
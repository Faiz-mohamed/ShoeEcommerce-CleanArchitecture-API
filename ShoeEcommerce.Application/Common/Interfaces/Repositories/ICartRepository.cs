using ShoeEcommerce.Domain.Entities;

namespace ShoeEcommerce.Application.Common.Interfaces.Repositories
{
    public interface ICartRepository
    {
        Task<Cart?> GetByUserIdAsync(Guid userId);
        Task AddAsync(Cart cart);
        Task SaveChangesAsync();
        Task DeleteCartAsync(Guid userId);
        Task ClearCartAsync(Guid userId);
    }
}
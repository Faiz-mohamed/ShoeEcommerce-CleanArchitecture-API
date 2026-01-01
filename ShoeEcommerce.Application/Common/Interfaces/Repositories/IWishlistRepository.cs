using ShoeEcommerce.Domain.Entities;

namespace ShoeEcommerce.Application.Common.Interfaces.Repositories
{
    public interface IWishlistRepository
    {
        Task<Wishlist?> GetByUserIdAsync(Guid userId);
        Task<bool> HasItemAsync(Guid userId, Guid productId);
        Task AddAsync(Wishlist wishlist);
        Task SaveChangesAsync();
    }
}
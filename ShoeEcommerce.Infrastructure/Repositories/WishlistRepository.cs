using Microsoft.EntityFrameworkCore;
using ShoeEcommerce.Application.Common.Interfaces.Repositories;
using ShoeEcommerce.Domain.Entities;
using ShoeEcommerce.Infrastructure.Data;

namespace ShoeEcommerce.Infrastructure.Repositories
{
    public class WishlistRepository : IWishlistRepository
    {
        private readonly AppDbContext _context;

        public WishlistRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Wishlist?> GetByUserIdAsync(Guid userId)
        {
            var wishlist = await _context.Wishlists
                .Include(w => w.Items)
                .ThenInclude(i => i.Product)
                    .ThenInclude(p => p.Brand)
                .Include(w => w.Items)
                    .ThenInclude(i => i.Product)
                        .ThenInclude(p => p.MainImageUrl)
                .FirstOrDefaultAsync(w => w.UserId == userId);

            if (wishlist != null)
            {
                wishlist.Items = wishlist.Items
                    .Where(i => !i.Product.IsDeleted && i.Product.Status)
                    .ToList();
            }

            return wishlist;
        }

        public async Task<bool> HasItemAsync(Guid userId, Guid productId)
        {
            return await _context.WishlistItems
                .AnyAsync(i => i.Wishlist.UserId == userId && i.ProductId == productId);
        }

        public async Task AddAsync(Wishlist wishlist)
        {
            await _context.Wishlists.AddAsync(wishlist);
            await _context.SaveChangesAsync();
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
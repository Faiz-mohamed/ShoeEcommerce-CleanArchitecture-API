using Microsoft.EntityFrameworkCore;
using ShoeEcommerce.Application.Interfaces.Repositories;
using ShoeEcommerce.Domain.Entities;
using ShoeEcommerce.Infrastructure.Data;

namespace ShoeEcommerce.Infrastructure.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly AppDbContext _context;

        public ProductRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Product?> GetByIdAsync(Guid productId)
        {
            return await _context.Products
                .AsNoTracking()
                .Where(p => !p.IsDeleted)
                .Include(p => p.Brand)
                .Include(p => p.ProductCategories)
                    .ThenInclude(pc => pc.Category)
                .Include(p => p.Variants.Where(v => !v.IsDeleted))
                    .ThenInclude(v => v.ProductImages)
                .FirstOrDefaultAsync(p => p.Id == productId);
        }

        public async Task<Product?> GetByVariantIdAsync(Guid variantId)
        {
            return await _context.Products
                .AsNoTracking()
                .Where(p => !p.IsDeleted)
                .Include(p => p.Brand)
                .Include(p => p.ProductCategories)
                    .ThenInclude(pc => pc.Category)
                .Include(p => p.Variants.Where(v => v.Id == variantId && !v.IsDeleted))
                    .ThenInclude(v => v.ProductImages)
                .FirstOrDefaultAsync(p => p.Variants.Any(v => v.Id == variantId && !v.IsDeleted));
        }

        public async Task<(List<Product> Items, int TotalCount)> GetPagedAsync(
            int pageNumber,
            int pageSize,
            Guid? categoryId = null,
            string? categorySlug = null,
            string? searchTerm = null)
        {

            var query = _context.Products
        .AsNoTracking()
        .Where(p => !p.IsDeleted)
        .Include(p => p.Brand)
        .Include(p => p.ProductCategories)
            .ThenInclude(pc => pc.Category)
        .Include(p => p.Variants.Where(v => !v.IsDeleted))
        .AsQueryable();

            if (categoryId.HasValue)
            {
                query = query.Where(p => p.ProductCategories.Any(pc => pc.CategoryId == categoryId));
            }

            if (!string.IsNullOrEmpty(categorySlug))
            {
                query = query.Where(p => p.ProductCategories.Any(pc => pc.Category.Slug == categorySlug));
            }

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(p => p.Name.Contains(searchTerm));
            }

            int totalCount = await query.CountAsync();

            var items = await query
                .OrderByDescending(p => p.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount);
        }

        public async Task<bool> IsNameUniqueAsync(string name)
        {
            return !await _context.Products.AnyAsync(p => p.Name.ToLower() == name.ToLower());
        }

        public async Task AddAsync(Product product)
        {
            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();
        }
    }
}
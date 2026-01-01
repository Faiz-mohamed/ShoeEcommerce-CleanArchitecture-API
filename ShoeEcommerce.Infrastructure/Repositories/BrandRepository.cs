using Microsoft.EntityFrameworkCore;
using ShoeEcommerce.Application.Common.Interfaces.Repositories;
using ShoeEcommerce.Domain.Entities;
using ShoeEcommerce.Infrastructure.Data;

namespace ShoeEcommerce.Infrastructure.Repositories
{
    public class BrandRepository : IBrandRepository
    {
        private readonly AppDbContext _context;

        public BrandRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Brand?> GetByIdAsync(Guid id)
        {
            return await _context.Brands.FirstOrDefaultAsync(b => b.Id == id);
        }

        public async Task<bool> IsNameUniqueAsync(string name)
        {
            return !await _context.Brands.AnyAsync(b => b.Name.ToLower() == name.ToLower());
        }

        public async Task AddAsync(Brand brand)
        {
            await _context.Brands.AddAsync(brand);
            await _context.SaveChangesAsync();
        }
    }
}
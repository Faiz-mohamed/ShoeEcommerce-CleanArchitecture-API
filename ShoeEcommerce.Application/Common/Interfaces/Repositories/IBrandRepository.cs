using ShoeEcommerce.Domain.Entities;

namespace ShoeEcommerce.Application.Common.Interfaces.Repositories
{
    public interface IBrandRepository
    {
        Task<Brand?> GetByIdAsync(Guid id);
        Task<bool> IsNameUniqueAsync(string name);
        Task AddAsync(Brand brand);
    }
}
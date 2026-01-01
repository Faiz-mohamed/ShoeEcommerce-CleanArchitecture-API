using ShoeEcommerce.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace ShoeEcommerce.Application.Common.Interfaces.Repositories
{
    public interface IRoleRepository
    {
        Task<IReadOnlyList<Role>> GetAllAsync();
        Task<Role?> GetByIdAsync(Guid id);
        Task<Role> AddAsync(Role role);
        Task UpdateAsync(Role role);
        Task DeleteAsync(Role role);
        Task<Role?> GetByNameAsync(string roleName);
    }
}

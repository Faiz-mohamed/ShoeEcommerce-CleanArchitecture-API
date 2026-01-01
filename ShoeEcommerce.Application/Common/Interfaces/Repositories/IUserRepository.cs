using ShoeEcommerce.Domain.Entities;

namespace ShoeEcommerce.Application.Common.Interfaces.Repositories;
public interface IUserRepository
{
    Task<User> AddAsync(User user);
    Task<User?> GetUserByIdAsync(Guid id);
    Task<User?> FindByNormalizedEmailAsync(string normalizedEmail);
    Task<User?> FindByNormalizedUsernameAsync(string normalizedUsername);
    Task<User?> FindByNormalizedPhoneAsync(string normalizedPhone);
    Task<bool> EmailExistsAsync(string normalizedEmail);
    Task<bool> UsernameExistsAsync(string normalizedUsername);
    Task<bool> PhoneExistsAsync(string normalizedPhone);
    Task UpdateAsync(User user);
    Task DeleteAsync(Guid id);
    Task<List<User>> GetAllAsync();
    Task AddUserBlockAsync(UserBlock userBlock);
}
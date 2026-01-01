using Microsoft.EntityFrameworkCore;
using ShoeEcommerce.Application.Common.Interfaces.Repositories;
using ShoeEcommerce.Domain.Entities;
using ShoeEcommerce.Infrastructure.Data;

namespace ShoeEcommerce.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _context;

    public UserRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<User> AddAsync(User user)
    {
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return user;
    }

    public async Task<User?> GetUserByIdAsync(Guid id)
    {
        return await _context.Users
            .Include(u => u.Role)
            .Include(u => u.UserBlocks)
            .FirstOrDefaultAsync(u => u.Id == id && !u.IsDeleted);
    }

    public async Task<User?> FindByNormalizedEmailAsync(string normalizedEmail)
    {
        return await _context.Users
            .Include(u => u.Role)
            .Include(u => u.UserBlocks)
            .FirstOrDefaultAsync(u => u.NormalizedEmail == normalizedEmail && !u.IsDeleted);
    }

    public async Task<User?> FindByNormalizedUsernameAsync(string normalizedUsername)
    {
        return await _context.Users
            .Include(u => u.Role)
            .Include(u => u.UserBlocks)
            .FirstOrDefaultAsync(u => u.NormalizedUsername == normalizedUsername && !u.IsDeleted);
    }

    public async Task<User?> FindByNormalizedPhoneAsync(string normalizedPhone)
    {
        return await _context.Users
            .Include(u => u.Role)
            .Include(u => u.UserBlocks)
            .FirstOrDefaultAsync(u => u.NormalizedPhoneNumber == normalizedPhone && !u.IsDeleted);
    }

    public async Task<bool> EmailExistsAsync(string normalizedEmail)
    {
        return await _context.Users
            .AnyAsync(u => u.NormalizedEmail == normalizedEmail && !u.IsDeleted);
    }

    public async Task<bool> UsernameExistsAsync(string normalizedUsername)
    {
        return await _context.Users
            .AnyAsync(u => u.NormalizedUsername == normalizedUsername && !u.IsDeleted);
    }

    public async Task<bool> PhoneExistsAsync(string normalizedPhone)
    {
        return await _context.Users
            .AnyAsync(u => u.NormalizedPhoneNumber == normalizedPhone && !u.IsDeleted);
    }

    public async Task UpdateAsync(User user)
    {
        _context.Users.Update(user);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var user = await GetUserByIdAsync(id);
        if (user != null)
        {
            user.IsDeleted = true;
            await UpdateAsync(user);
        }
    }

    public async Task<List<User>> GetAllAsync()
    {
        return await _context.Users
            .AsNoTracking()
            .Include(u => u.Role)
            .Include(u => u.UserBlocks)
            .Where(u => !u.IsDeleted)
            .OrderByDescending(u => u.CreatedAt)
            .ToListAsync();
    }

    public async Task AddUserBlockAsync(UserBlock userBlock)
    {
        await _context.UserBlocks.AddAsync(userBlock);
        await _context.SaveChangesAsync();
    }
}
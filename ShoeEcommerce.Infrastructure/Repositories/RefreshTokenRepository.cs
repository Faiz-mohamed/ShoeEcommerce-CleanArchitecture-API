using Microsoft.EntityFrameworkCore;
using ShoeEcommerce.Application.Common.Interfaces.Repositories;
using ShoeEcommerce.Domain.Entities;
using ShoeEcommerce.Infrastructure.Data;

namespace ShoeEcommerce.Infrastructure.Repositories;
public class RefreshTokenRepository : IRefreshTokenRepository
{
    private readonly AppDbContext _context;

    public RefreshTokenRepository(AppDbContext context)
    {
        _context = context;
    }
    public async Task<RefreshToken> AddAsync(RefreshToken refreshToken)
    {
        _context.RefreshTokens.Add(refreshToken);
        await _context.SaveChangesAsync();
        return refreshToken;
    }

    public async Task<RefreshToken?> FindByTokenAsync(string token)
    {
        return await _context.RefreshTokens
            .Include(rt => rt.User)
                .ThenInclude(u => u.Role)
            .Include(rt => rt.User)
                .ThenInclude(u => u.UserBlocks)
            .FirstOrDefaultAsync(rt => rt.Token == token);
    }

    public async Task<List<RefreshToken>> GetActiveTokensByUserAndIpAsync(Guid userId, string ipAddress)
    {
        return await _context.RefreshTokens
            .Where(rt =>
                rt.UserId == userId &&
                rt.CreatedByIp == ipAddress &&
                rt.RevokedAt == null &&
                rt.ExpiresAt > DateTime.UtcNow)
            .OrderByDescending(rt => rt.CreatedAt)
            .ToListAsync();
    }

    public async Task UpdateAsync(RefreshToken refreshToken)
    {
        _context.RefreshTokens.Update(refreshToken);
        await _context.SaveChangesAsync();
    }

    public async Task RevokeAllUserTokensAsync(Guid userId, string reason)
    {
        var activeTokens = await _context.RefreshTokens
            .Where(rt =>
                rt.UserId == userId &&
                rt.RevokedAt == null)
            .ToListAsync();

        foreach (var token in activeTokens)
        {
            token.RevokedAt = DateTime.UtcNow;
            token.ReasonRevoked = reason;
        }

        await _context.SaveChangesAsync();
    }

    public async Task<int> DeleteOldTokensAsync(DateTime cutoffDate)
    {
        var oldTokens = await _context.RefreshTokens
            .Where(rt =>
                (rt.RevokedAt != null && rt.RevokedAt < cutoffDate) ||
                (rt.ExpiresAt < cutoffDate))
            .ToListAsync();

        var count = oldTokens.Count;

        if (count > 0)
        {
            _context.RefreshTokens.RemoveRange(oldTokens);
            await _context.SaveChangesAsync();
        }

        return count;
    }
}
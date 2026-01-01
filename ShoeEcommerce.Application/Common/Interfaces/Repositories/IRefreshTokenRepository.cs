using ShoeEcommerce.Domain.Entities;

namespace ShoeEcommerce.Application.Common.Interfaces.Repositories;
public interface IRefreshTokenRepository
{
    Task<RefreshToken> AddAsync(RefreshToken refreshToken);
    Task<RefreshToken?> FindByTokenAsync(string token);
    Task<List<RefreshToken>> GetActiveTokensByUserAndIpAsync(Guid userId, string ipAddress);
    Task UpdateAsync(RefreshToken refreshToken);
    Task RevokeAllUserTokensAsync(Guid userId, string reason);
    Task<int> DeleteOldTokensAsync(DateTime cutoffDate);
}
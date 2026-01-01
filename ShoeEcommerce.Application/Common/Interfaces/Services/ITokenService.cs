using ShoeEcommerce.Domain.Entities;

namespace ShoeEcommerce.Application.Common.Interfaces.Services;
public interface ITokenService
{
    string GenerateAccessToken(User user);
    string GenerateRefreshToken();
    int GetAccessTokenExpirySeconds();
}
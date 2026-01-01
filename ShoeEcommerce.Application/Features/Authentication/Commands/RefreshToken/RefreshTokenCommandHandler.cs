using ShoeEcommerce.Application.Common.Exceptions;
using ShoeEcommerce.Application.Common.Interfaces.Repositories;
using ShoeEcommerce.Application.Common.Interfaces.Services;
using ShoeEcommerce.Application.Features.Authentication.DTOs;

namespace ShoeEcommerce.Application.Features.Authentication.Commands.RefreshToken;
public class RefreshTokenCommandHandler
{
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IUserRepository _userRepository;
    private readonly ITokenService _tokenService;

    public RefreshTokenCommandHandler(
        IRefreshTokenRepository refreshTokenRepository,
        IUserRepository userRepository,
        ITokenService tokenService)
    {
        _refreshTokenRepository = refreshTokenRepository;
        _userRepository = userRepository;
        _tokenService = tokenService;
    }

    public async Task<AuthResponse> Handle(RefreshTokenCommand command)
    {

        var refreshToken = await _refreshTokenRepository.FindByTokenAsync(command.RefreshToken);

        if (refreshToken == null)
        {
            throw new InvalidRefreshTokenException("Invalid refresh token");
        }

        if (refreshToken.RevokedAt.HasValue)
        {

            if (!string.IsNullOrEmpty(refreshToken.ReplacementToken))
            {
                await _refreshTokenRepository.RevokeAllUserTokensAsync(
                    refreshToken.UserId,
                    "Token reuse detected - possible security breach"
                );

                // TODO: Log security event
                // TODO: Send email to user about suspicious activity
                // await _emailService.SendSecurityAlertEmail(refreshToken.User.Email);

                throw new InvalidRefreshTokenException(
                    "Token reuse detected. All your refresh tokens have been revoked for security. Please login again."
                );
            }

            throw new InvalidRefreshTokenException("Refresh token has been revoked. Please login again.");
        }

        if (refreshToken.IsExpired)
        {
            throw new InvalidRefreshTokenException("Refresh token has expired. Please login again.");
        }


        var user = refreshToken.User;  // Loaded via Include in repository

        if (user == null)
        {
            throw new NotFoundException("User", refreshToken.UserId);
        }

        if (user.IsDeleted)
        {
            throw new UnauthorizedException("Account has been deleted");
        }

        var activeBlock = user.UserBlocks?.FirstOrDefault(b => b.IsActive);
        if (activeBlock != null)
        {
            throw new AccountBlockedException(
                "Your account has been blocked",
                activeBlock.Reason,
                activeBlock.ExpiresAt
            );
        }

        if (!string.IsNullOrEmpty(refreshToken.SecurityStamp) &&
            refreshToken.SecurityStamp != user.SecurityStamp)
        {
            throw new InvalidRefreshTokenException(
                "Session has Expired. Please login again."
            );
        }

        var newAccessToken = _tokenService.GenerateAccessToken(user);

        var newRefreshTokenString = _tokenService.GenerateRefreshToken();

        var originalDuration = refreshToken.ExpiresAt - refreshToken.CreatedAt;
        var newExpiry = DateTime.UtcNow.Add(originalDuration);

        var newRefreshToken = new Domain.Entities.RefreshToken
        {
            UserId = user.Id,
            Token = newRefreshTokenString,
            ExpiresAt = newExpiry,
            CreatedByIp = command.IpAddress,
            SecurityStamp = user.SecurityStamp
        };

        refreshToken.RevokedAt = DateTime.UtcNow;
        refreshToken.RevokedByIp = command.IpAddress;
        refreshToken.ReasonRevoked = "Token rotation - replaced with new token";
        refreshToken.ReplacementToken = newRefreshTokenString;

        await _refreshTokenRepository.UpdateAsync(refreshToken);

        if (!string.IsNullOrEmpty(command.IpAddress))
        {
            var existingTokens = await _refreshTokenRepository
                .GetActiveTokensByUserAndIpAsync(user.Id, command.IpAddress);

            if (existingTokens.Count >= 3)
            {
                var tokensToRevoke = existingTokens
                    .OrderByDescending(t => t.CreatedAt)
                    .Skip(2)
                    .ToList();

                foreach (var oldToken in tokensToRevoke)
                {
                    oldToken.RevokedAt = DateTime.UtcNow;
                    oldToken.ReasonRevoked = "Exceeded device token limit";
                    await _refreshTokenRepository.UpdateAsync(oldToken);
                }
            }
        }

        await _refreshTokenRepository.AddAsync(newRefreshToken);

        return new AuthResponse
        {
            AccessToken = newAccessToken,
            RefreshToken = newRefreshTokenString,
            TokenType = "Bearer",
            ExpiresIn = _tokenService.GetAccessTokenExpirySeconds(),
            User = new UserDto
            {
                Id = user.Id,
                Email = user.Email,
                FullName = user.FullName,
                Username = user.Username,
                Role = user.Role?.Name ?? "customer",
                EmailConfirmed = user.EmailConfirmed,
                PhoneNumberConfirmed = user.PhoneNumberConfirmed,
                PhoneNumber = user.PhoneNumber,
                CreatedAt = user.CreatedAt
            }
        };
    }
}
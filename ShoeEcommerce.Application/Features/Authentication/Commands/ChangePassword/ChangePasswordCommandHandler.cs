using ShoeEcommerce.Application.Common.Exceptions;
using ShoeEcommerce.Application.Common.Interfaces.Repositories;
using ShoeEcommerce.Application.Common.Interfaces.Services;
using ShoeEcommerce.Application.Features.Authentication.DTOs;
using ShoeEcommerce.Domain.Entities;

namespace ShoeEcommerce.Application.Features.Authentication.Commands.ChangePassword
{
    public class ChangePasswordCommandHandler
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly ITokenService _tokenService;
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly ICurrentUserService _currentUserService;

        public ChangePasswordCommandHandler(
            IUserRepository userRepository,
            IPasswordHasher passwordHasher,
            ITokenService tokenService,
            IRefreshTokenRepository refreshTokenRepository,
            ICurrentUserService currentUserService)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _tokenService = tokenService;
            _refreshTokenRepository = refreshTokenRepository;
            _currentUserService = currentUserService;
        }

        public async Task<AuthResponse> Handle(ChangePasswordRequest request)
        {
            var userIdString = _currentUserService.UserId;
            var ipAddress = _currentUserService.IpAddress ?? "Unknown";

            if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var userId))
            {
                throw new UnauthorizedException("User identity could not be verified.");
            }

            var user = await _userRepository.GetUserByIdAsync(userId);
            if (user == null) throw new NotFoundException("User not found.");

            if (user.IsDeleted)
            {
                throw new UnauthorizedException("Account has been deleted.");
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

            if (!_passwordHasher.VerifyPassword(request.CurrentPassword, user.PasswordHash))
            {
                throw new ValidationException("Current password is incorrect.");
            }

            user.PasswordHash = _passwordHasher.HashPassword(request.NewPassword);

            user.SecurityStamp = Guid.NewGuid().ToString();

            var accessToken = _tokenService.GenerateAccessToken(user);
            var refreshToken = _tokenService.GenerateRefreshToken();


            if (ipAddress != "Unknown")
            {
                var existingTokens = await _refreshTokenRepository
                    .GetActiveTokensByUserAndIpAsync(user.Id, ipAddress);

                if (existingTokens.Count >= 1)
                {
                    var tokensToRevoke = existingTokens
                        .OrderByDescending(t => t.CreatedAt)
                        .ToList();

                    foreach (var oldToken in tokensToRevoke)
                    {
                        oldToken.RevokedAt = DateTime.UtcNow;
                        oldToken.RevokedByIp = ipAddress;
                        oldToken.ReasonRevoked = "Exceeded device token limit during password change";
                        await _refreshTokenRepository.UpdateAsync(oldToken);
                    }
                }
            }

            var refreshTokenEntity = new ShoeEcommerce.Domain.Entities.RefreshToken
            {
                Id = Guid.NewGuid(),
                Token = refreshToken,
                UserId = user.Id,
                ExpiresAt = DateTime.UtcNow.AddDays(7),
                CreatedByIp = ipAddress,
                CreatedAt = DateTime.UtcNow,
                SecurityStamp = user.SecurityStamp
            };

            await _userRepository.UpdateAsync(user);
            await _refreshTokenRepository.AddAsync(refreshTokenEntity);

            return new AuthResponse
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                TokenType = "Bearer",
                ExpiresIn = _tokenService.GetAccessTokenExpirySeconds(),
                User = new UserDto
                {
                    Id = user.Id,
                    Email = user.Email,
                    FullName = user.FullName,
                    Username = user.Username,
                    Role = user.Role?.Name ?? "Customer",
                    EmailConfirmed = user.EmailConfirmed,
                    PhoneNumberConfirmed = user.PhoneNumberConfirmed,
                    PhoneNumber = user.PhoneNumber,
                    CreatedAt = user.CreatedAt
                }
            };
        }
    }
}
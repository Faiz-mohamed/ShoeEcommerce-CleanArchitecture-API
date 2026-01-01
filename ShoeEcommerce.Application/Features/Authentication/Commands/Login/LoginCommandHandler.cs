using ShoeEcommerce.Application.Common.Exceptions;
using ShoeEcommerce.Application.Common.Helpers;
using ShoeEcommerce.Application.Common.Interfaces.Repositories;
using ShoeEcommerce.Application.Common.Interfaces.Services;
using ShoeEcommerce.Application.Features.Authentication.DTOs;
using ShoeEcommerce.Domain.Entities;

namespace ShoeEcommerce.Application.Features.Authentication.Commands.Login;
public class LoginCommandHandler
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ITokenService _tokenService;
    private readonly IRefreshTokenRepository _refreshTokenRepository;

    private const int MaxFailedAccessAttempts = 5;
    private const int LockoutDurationMinutes = 15;

    public LoginCommandHandler(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        ITokenService tokenService,
        IRefreshTokenRepository refreshTokenRepository)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _tokenService = tokenService;
        _refreshTokenRepository = refreshTokenRepository;
    }

    public async Task<AuthResponse> Handle(LoginCommand command)
    {

        User? user = await FindUserByIdentifier(command.Identifier);

        const string invalidCredentialsMessage = "Invalid email, username, phone, or password";

        if (user == null)
        {
            throw new UnauthorizedException(invalidCredentialsMessage);
        }

        if (user.IsDeleted)
        {
            throw new UnauthorizedException("Account has been deleted");
        }

        if (user.LockoutEndAt.HasValue && user.LockoutEndAt.Value > DateTime.UtcNow)
        {
            var remainingTime = user.LockoutEndAt.Value - DateTime.UtcNow;
            throw new AccountLockedException(
                $"Account is locked due to multiple failed login attempts. Try again in {Math.Ceiling(remainingTime.TotalMinutes)} minutes.",
                user.LockoutEndAt.Value
            );
        }

        if (user.LockoutEndAt.HasValue && user.LockoutEndAt.Value <= DateTime.UtcNow)
        {
            user.LockoutEndAt = null;
            user.AccessFailedCount = 0;
        }

        var activeBlock = user.UserBlocks.FirstOrDefault(b => b.IsActive);
        if (activeBlock != null)
        {
            var message = !string.IsNullOrEmpty(activeBlock.Reason)
                ? $"Your account has been blocked. Reason: {activeBlock.Reason}"
                : "Your account has been blocked by an administrator";

            throw new AccountBlockedException(
                message,
                activeBlock.Reason,
                activeBlock.ExpiresAt
            );
        }

        bool isPasswordValid = _passwordHasher.VerifyPassword(
            command.Password,
            user.PasswordHash
        );

        if (!isPasswordValid)
        {
            await HandleFailedLogin(user);

            throw new UnauthorizedException(invalidCredentialsMessage);
        }

        if (user.AccessFailedCount > 0)
        {
            user.AccessFailedCount = 0;
            user.LockoutEndAt = null;
            await _userRepository.UpdateAsync(user);
        }

        // ===== STEP 5: CHECK EMAIL CONFIRMATION (Optional) =====

        // if (!user.EmailConfirmed)
        // {
        //     throw new EmailNotConfirmedException(
        //         "Please confirm your email address before logging in. Check your inbox for confirmation email."
        //     );
        // }

        var accessToken = _tokenService.GenerateAccessToken(user);

        var refreshTokenString = _tokenService.GenerateRefreshToken();

        var refreshTokenExpiry = command.RememberMe
            ? DateTime.UtcNow.AddDays(7)
            : DateTime.UtcNow.AddHours(24);

        var refreshToken = new Domain.Entities.RefreshToken
        {
            UserId = user.Id,
            Token = refreshTokenString,
            ExpiresAt = refreshTokenExpiry,
            CreatedByIp = command.IpAddress,
            SecurityStamp = user.SecurityStamp
        };


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
                    oldToken.ReasonRevoked = "Exceeded device token limit - keeping only 3 most recent";
                    await _refreshTokenRepository.UpdateAsync(oldToken);
                }
            }
        }

        await _refreshTokenRepository.AddAsync(refreshToken);

        return new AuthResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshTokenString,
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

    private async Task<User?> FindUserByIdentifier(string identifier)
    {

        if (identifier.Contains("@"))
        {
            var normalizedEmail = identifier.ToUpperInvariant();
            return await _userRepository.FindByNormalizedEmailAsync(normalizedEmail);
        }

        if (identifier.StartsWith("+") || identifier.All(char.IsDigit))
        {
            var normalizedPhone = PhoneNumberHelper.Normalize(identifier);
            if (normalizedPhone != null)
            {
                return await _userRepository.FindByNormalizedPhoneAsync(normalizedPhone);
            }
        }

        var normalizedUsername = identifier.ToUpperInvariant();
        return await _userRepository.FindByNormalizedUsernameAsync(normalizedUsername);
    }

    private async Task HandleFailedLogin(User user)
    {
        user.AccessFailedCount++;

        if (user.AccessFailedCount >= MaxFailedAccessAttempts)
        {
            user.LockoutEndAt = DateTime.UtcNow.AddMinutes(LockoutDurationMinutes);

            // TODO: Log security event
            // TODO: Send email notification to user about lockout
            // await _emailService.SendAccountLockedEmail(user.Email, user.LockoutEndAt.Value);
        }

        await _userRepository.UpdateAsync(user);
    }
}
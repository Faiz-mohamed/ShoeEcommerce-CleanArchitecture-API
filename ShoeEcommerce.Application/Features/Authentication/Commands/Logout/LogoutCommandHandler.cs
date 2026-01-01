using ShoeEcommerce.Application.Common.Exceptions;
using ShoeEcommerce.Application.Common.Interfaces.Repositories;

namespace ShoeEcommerce.Application.Features.Authentication.Commands.Logout;
public class LogoutCommandHandler
{
    private readonly IRefreshTokenRepository _refreshTokenRepository;

    public LogoutCommandHandler(IRefreshTokenRepository refreshTokenRepository)
    {
        _refreshTokenRepository = refreshTokenRepository;
    }

    public async Task Handle(LogoutCommand command)
    {

        var refreshToken = await _refreshTokenRepository.FindByTokenAsync(command.RefreshToken);

        if (refreshToken == null)
        {
            return;
        }

        if (refreshToken.RevokedAt.HasValue)
        {
            return;
        }

        refreshToken.RevokedAt = DateTime.UtcNow;
        refreshToken.RevokedByIp = command.IpAddress;
        refreshToken.ReasonRevoked = "User logged out";

        await _refreshTokenRepository.UpdateAsync(refreshToken);


        // TODO: Log logout event
        // await _auditLogRepository.AddAsync(new AuditLog
        // {
        //     ActorId = refreshToken.UserId,
        //     Action = "UserLoggedOut",
        //     Details = $"User logged out from IP: {command.IpAddress}"
        // });
        return;
    }
}
namespace ShoeEcommerce.Application.Features.Authentication.Commands.RefreshToken;
public class RefreshTokenCommand
{
    public string RefreshToken { get; set; } = string.Empty;
    public string? IpAddress { get; set; }
}
namespace ShoeEcommerce.Application.Features.Authentication.Commands.Logout;
public class LogoutCommand
{
    public string RefreshToken { get; set; } = string.Empty;
    public string? IpAddress { get; set; }
}
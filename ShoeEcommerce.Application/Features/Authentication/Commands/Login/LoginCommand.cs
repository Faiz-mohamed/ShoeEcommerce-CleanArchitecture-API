namespace ShoeEcommerce.Application.Features.Authentication.Commands.Login;

public class LoginCommand
{
    public string Identifier { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public bool RememberMe { get; set; } = false;
    public string? IpAddress { get; set; }
}
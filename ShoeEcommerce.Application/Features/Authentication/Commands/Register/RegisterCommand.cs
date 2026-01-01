using ShoeEcommerce.Application.Features.Authentication.DTOs;

namespace ShoeEcommerce.Application.Features.Authentication.Commands.Register;

public class RegisterCommand
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string ConfirmPassword { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string? Username { get; set; }
    public string? PhoneNumber { get; set; }
    public string? PhoneCountryCode { get; set; }
    public string? IpAddress { get; set; }
}
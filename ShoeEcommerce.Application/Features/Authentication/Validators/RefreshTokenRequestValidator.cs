using FluentValidation;
using ShoeEcommerce.Application.Features.Authentication.DTOs;

namespace ShoeEcommerce.Application.Features.Authentication.Validators;
public class RefreshTokenRequestValidator : AbstractValidator<RefreshTokenRequest>
{
    public RefreshTokenRequestValidator()
    {
        RuleFor(x => x.RefreshToken)
            .NotEmpty()
            .WithMessage("Refresh token is required")
            .MinimumLength(50)
            .WithMessage("Invalid refresh token format")
            .MaximumLength(500)
            .WithMessage("Invalid refresh token format");
    }
}
using FluentValidation;
using ShoeEcommerce.Application.Features.Authentication.DTOs;

namespace ShoeEcommerce.Application.Features.Authentication.Validators;
public class LoginRequestValidator : AbstractValidator<LoginRequest>
{
    public LoginRequestValidator()
    {

        RuleFor(x => x.Identifier)
            .NotEmpty()
            .WithMessage("Email, username, or phone number is required")
            .MinimumLength(3)
            .WithMessage("Identifier must be at least 3 characters")
            .MaximumLength(256)
            .WithMessage("Identifier must not exceed 256 characters");

        RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage("Password is required");
    }
}
using FluentValidation;
using ShoeEcommerce.Application.Common.Helpers;
using ShoeEcommerce.Application.Features.Authentication.DTOs;

namespace ShoeEcommerce.Application.Features.Authentication.Validators;

public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
{
    public RegisterRequestValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("Email is required")
            .EmailAddress()
            .WithMessage("Invalid email format")
            .MaximumLength(256)
            .WithMessage("Email must not exceed 256 characters");

        RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage("Password is required")
            .MinimumLength(8)
            .WithMessage("Password must be at least 8 characters long")
            .Matches(@"[A-Z]")
            .WithMessage("Password must contain at least one uppercase letter")
            .Matches(@"[a-z]")
            .WithMessage("Password must contain at least one lowercase letter")
            .Matches(@"[0-9]")
            .WithMessage("Password must contain at least one number")
            .Matches(@"[@$!%*?&#]")
            .WithMessage("Password must contain at least one special character (@$!%*?&#)");

        RuleFor(x => x.ConfirmPassword)
            .NotEmpty()
            .WithMessage("Please confirm your password")
            .Equal(x => x.Password)
            .WithMessage("Passwords do not match");

        RuleFor(x => x.FullName)
            .NotEmpty()
            .WithMessage("Full name is required")
            .MinimumLength(2)
            .WithMessage("Full name must be at least 2 characters")
            .MaximumLength(200)
            .WithMessage("Full name must not exceed 200 characters");


        RuleFor(x => x.Username)
            .MinimumLength(3)
            .WithMessage("Username must be at least 3 characters")
            .MaximumLength(50)
            .WithMessage("Username must not exceed 50 characters")
            .Matches(@"^[a-zA-Z0-9_-]+$")
            .WithMessage("Username can only contain letters, numbers, underscores, and hyphens")
            .Must(username => !username.StartsWith("_") && !username.StartsWith("-"))
            .WithMessage("Username cannot start with underscore or hyphen")
            .Must(username => !username.EndsWith("_") && !username.EndsWith("-"))
            .WithMessage("Username cannot end with underscore or hyphen")
            .When(x => !string.IsNullOrWhiteSpace(x.Username));


        RuleFor(x => x.PhoneNumber)
            .Must((request, phone) =>
            {
                if (phone!.StartsWith("+"))
                {
                    return PhoneNumberHelper.IsValid(phone);
                }

                var country = request.PhoneCountryCode ?? PhoneNumberHelper.DefaultCountry;
                return PhoneNumberHelper.IsValid(phone, country);
            })
            .WithMessage("Invalid phone number format")
            .When(x => !string.IsNullOrWhiteSpace(x.PhoneNumber));

        RuleFor(x => x.PhoneCountryCode)
            .Matches(@"^[A-Z]{2}$")
            .WithMessage("Country code must be a valid 2-letter ISO code (e.g., IN, US, GB)")
            .When(x => !string.IsNullOrWhiteSpace(x.PhoneNumber) &&
                      !string.IsNullOrWhiteSpace(x.PhoneCountryCode));
    }
}
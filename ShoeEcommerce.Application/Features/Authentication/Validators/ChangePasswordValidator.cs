using FluentValidation;
using ShoeEcommerce.Application.Features.Authentication.DTOs;

namespace ShoeEcommerce.Application.Features.Authentication.Validators
{
    public class ChangePasswordValidator : AbstractValidator<ChangePasswordRequest>
    {
        public ChangePasswordValidator()
        {
            RuleFor(x => x.CurrentPassword)
                .NotEmpty().WithMessage("Current password is required.");

            RuleFor(x => x.NewPassword)
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
                .WithMessage("Password must contain at least one special character (@$!%*?&#)")
                .NotEqual(x => x.CurrentPassword)
                .WithMessage("New password cannot be the same as the old password.");

            RuleFor(x => x.ConfirmNewPassword)
                .NotEmpty()
                .WithMessage("Please confirm your password")
                .Equal(x => x.NewPassword)
                .WithMessage("Passwords do not match");
        }
    }
}
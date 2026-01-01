using FluentValidation;
using ShoeEcommerce.Application.Features.Users.DTOs;

namespace ShoeEcommerce.Application.Features.Users.Validators
{
    public class BlockUserValidator : AbstractValidator<BlockUserRequest>
    {
        public BlockUserValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("User ID is required.");

            RuleFor(x => x.Reason)
                .NotEmpty().WithMessage("You must provide a reason for blocking this user.")
                .MinimumLength(5).WithMessage("Reason must be at least 5 characters long.");

            RuleFor(x => x.ExpiresInDays)
                .GreaterThan(0)
                .When(x => x.ExpiresInDays.HasValue)
                .WithMessage("Expiration days must be greater than 0.");
        }
    }
}
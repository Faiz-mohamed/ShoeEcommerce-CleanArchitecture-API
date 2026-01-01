using FluentValidation;
using ShoeEcommerce.Application.Features.Brands.DTOs;

namespace ShoeEcommerce.Application.Features.Brands.Validators
{
    public class CreateBrandValidator : AbstractValidator<CreateBrandRequest>
    {
        public CreateBrandValidator()
        {
            RuleFor(p => p.Name)
                .NotEmpty().WithMessage("Brand name is required.")
                .MaximumLength(100).WithMessage("Brand name must not exceed 100 characters.");
        }
    }
}
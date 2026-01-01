using FluentValidation;
using ShoeEcommerce.Application.Features.Cart.DTOs;

namespace ShoeEcommerce.Application.Features.Cart.Validators
{
    public class AddToCartValidator : AbstractValidator<AddToCartRequest>
    {
        public AddToCartValidator()
        {
            RuleFor(x => x.ProductVariantId)
                .NotEmpty().WithMessage("Product Variant ID is required.");

            RuleFor(x => x.Quantity)
                .GreaterThan(0).WithMessage("Quantity must be at least 1.");
        }
    }
}
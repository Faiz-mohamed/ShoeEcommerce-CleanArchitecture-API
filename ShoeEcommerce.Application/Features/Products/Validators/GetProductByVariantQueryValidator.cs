using FluentValidation;
using ShoeEcommerce.Application.Features.Product.Queries.GetProductByVariant;

namespace ShoeEcommerce.Application.Features.Products.Validators
{
    public class GetProductByVariantQueryValidator : AbstractValidator<GetProductByVariantQuery>
    {
        public GetProductByVariantQueryValidator()
        {
            RuleFor(x => x.VariantId)
                .NotEmpty().WithMessage("Variant ID is required.")
                .NotEqual(Guid.Empty).WithMessage("Invalid Variant ID.");
        }
    }
}
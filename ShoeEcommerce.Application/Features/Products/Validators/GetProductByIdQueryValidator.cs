using FluentValidation;
using ShoeEcommerce.Application.Features.Product.Queries.GetProductById;

namespace ShoeEcommerce.Application.Features.Products.Validators
{
    public class GetProductByIdQueryValidator : AbstractValidator<GetProductByIdQuery>
    {
        public GetProductByIdQueryValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Product ID is required.")
                .NotEqual(Guid.Empty).WithMessage("Invalid Product ID.");
        }
    }
}
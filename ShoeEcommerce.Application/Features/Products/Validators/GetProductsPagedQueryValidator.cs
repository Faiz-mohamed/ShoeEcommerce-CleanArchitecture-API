using FluentValidation;
using ShoeEcommerce.Application.Features.Product.Queries.GetProductsPaged;

namespace ShoeEcommerce.Application.Features.Products.Validators
{
    public class GetProductsPagedQueryValidator : AbstractValidator<GetProductsPagedQuery>
    {
        public GetProductsPagedQueryValidator()
        {
            RuleFor(x => x.PageNumber)
                .GreaterThan(0).WithMessage("Page number must be at least 1.");

            RuleFor(x => x.PageSize)
                .GreaterThan(0).WithMessage("Page size must be greater than 0.")
                .LessThanOrEqualTo(50).WithMessage("Page size cannot exceed 50 items.");

            RuleFor(x => x.CategoryId)
                .Must(id => id != Guid.Empty)
                .When(x => x.CategoryId.HasValue)
                .WithMessage("Category ID cannot be an empty GUID.");

            RuleFor(x => x.CategorySlug)
                .NotEmpty().WithMessage("Category slug cannot be empty.")
                .MaximumLength(200).WithMessage("Category slug is too long.")
                .When(x => !string.IsNullOrEmpty(x.CategorySlug));

            RuleFor(x => x.SearchTerm)
                .MaximumLength(100).WithMessage("Search term cannot exceed 100 characters.")
                .When(x => !string.IsNullOrEmpty(x.SearchTerm));
        }
    }
}
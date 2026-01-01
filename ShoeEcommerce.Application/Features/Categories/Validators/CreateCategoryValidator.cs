using FluentValidation;
using ShoeEcommerce.Application.Features.Categories.DTOs;

namespace ShoeEcommerce.Application.Features.Categories.Validators
{
    public class CreateCategoryValidator : AbstractValidator<CreateCategoryRequest>
    {
        public CreateCategoryValidator()
        {
            RuleFor(p => p.Name)
                .NotEmpty().WithMessage("Category name is required.")
                .MaximumLength(100).WithMessage("Category name must not exceed 100 characters.");

            // We don't validate ParentId here because that requires a DB check (Async).
            // That happens in the Handler.
        }
    }
}
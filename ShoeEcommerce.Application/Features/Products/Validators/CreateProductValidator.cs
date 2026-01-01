using FluentValidation;
using ShoeEcommerce.Application.Features.Products.DTOs;

namespace ShoeEcommerce.Application.Features.Products.Validators
{
    public class CreateProductValidator : AbstractValidator<CreateProductRequest>
    {
        public CreateProductValidator()
        {
            RuleFor(p => p.Name)
                .NotEmpty().WithMessage("Product Name is required.")
                .MaximumLength(200);

            RuleFor(p => p.MainImageUrl)
                .NotEmpty().WithMessage("Main Image URL is required.");


            RuleFor(p => p.BrandId)
                .NotEqual(Guid.Empty).When(p => p.BrandId.HasValue)
                .WithMessage("Invalid Brand ID.");

            RuleFor(p => p.Variants)
                .NotEmpty().WithMessage("At least one product variant is required.");

            RuleForEach(p => p.Variants).SetValidator(new ProductVariantValidator());
        }
    }

    public class ProductVariantValidator : AbstractValidator<CreateProductVariantDto>
    {
        public ProductVariantValidator()
        {
            RuleFor(v => v.Sku).NotEmpty().WithMessage("SKU is required.");
            RuleFor(v => v.Price).GreaterThan(0).WithMessage("Price must be greater than 0.");
            RuleFor(v => v.InventoryQty).GreaterThanOrEqualTo(0).WithMessage("Inventory cannot be negative.");
        }
    }
}
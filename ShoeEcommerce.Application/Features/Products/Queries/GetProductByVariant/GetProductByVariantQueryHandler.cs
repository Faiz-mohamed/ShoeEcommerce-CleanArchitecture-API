using FluentValidation;
using ShoeEcommerce.Application.Common.Exceptions;
using ShoeEcommerce.Application.Features.Products.DTOs;
using ShoeEcommerce.Application.Interfaces.Repositories;
using System.Text.Json;

namespace ShoeEcommerce.Application.Features.Product.Queries.GetProductByVariant
{
    public class GetProductByVariantQueryHandler
    {
        private readonly IProductRepository _productRepository;
        private readonly IValidator<GetProductByVariantQuery> _validator;

        public GetProductByVariantQueryHandler(
            IProductRepository productRepository,
            IValidator<GetProductByVariantQuery> validator)
        {
            _productRepository = productRepository;
            _validator = validator;
        }

        public async Task<ProductDetailDto?> HandleAsync(GetProductByVariantQuery query)
        {
            var validationResult = await _validator.ValidateAsync(query);
            if (!validationResult.IsValid)
            {
                throw new ShoeEcommerce.Application.Common.Exceptions.ValidationException(validationResult.ToDictionary());
            }

            var product = await _productRepository.GetByVariantIdAsync(query.VariantId);

            if (product == null)
            {
                throw new NotFoundException("ProductVariant", query.VariantId);
            }

            return new ProductDetailDto
            {
                Id = product.Id,
                Name = product.Name,
                Slug = product.Slug,
                BrandName = product.Brand?.Name ?? "",
                MainImageUrl = product.MainImageUrl,
                Description = product.Description,
                PriceStart = product.Variants.Any() ? product.Variants.Min(v => v.Price) : 0,
                CategoryName = product.ProductCategories.FirstOrDefault()?.Category?.Name ?? string.Empty,

                Variants = product.Variants.Select(v => {
                    var variantImageEntry = v.ProductImages.FirstOrDefault();
                    return new ProductVariantDto
                    {
                        Id = v.Id,
                        Sku = v.Sku,
                        Size = v.Size ?? "",
                        Color = v.Colour ?? "",
                        Price = v.Price,
                        StockQuantity = v.InventoryQty,
                        Images = ParseImages(variantImageEntry?.ImagesJson)
                    };
                }).ToList()
            };
        }

        private List<string> ParseImages(string? json)
        {
            if (string.IsNullOrWhiteSpace(json)) return new List<string>();
            try { return JsonSerializer.Deserialize<List<string>>(json) ?? new List<string>(); }
            catch { return new List<string>(); }
        }
    }
}
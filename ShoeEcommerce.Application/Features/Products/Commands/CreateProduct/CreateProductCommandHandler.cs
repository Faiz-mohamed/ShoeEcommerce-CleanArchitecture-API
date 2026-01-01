using FluentValidation;
using ShoeEcommerce.Application.Common.Interfaces.Repositories;
using ShoeEcommerce.Application.Features.Products.DTOs;
using ShoeEcommerce.Application.Interfaces.Repositories;
using ShoeEcommerce.Domain.Entities;
using System.Text.Json;
using ValidationException = ShoeEcommerce.Application.Common.Exceptions.ValidationException;

namespace ShoeEcommerce.Application.Features.Products.Commands.CreateProduct
{
    public class CreateProductCommandHandler
    {
        private readonly IProductRepository _productRepository;
        private readonly IBrandRepository _brandRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IValidator<CreateProductRequest> _validator;

        public CreateProductCommandHandler(
            IProductRepository productRepository,
            IBrandRepository brandRepository,
            ICategoryRepository categoryRepository,
            IValidator<CreateProductRequest> validator)
        {
            _productRepository = productRepository;
            _brandRepository = brandRepository;
            _categoryRepository = categoryRepository;
            _validator = validator;
        }

        public async Task<Guid> Handle(CreateProductRequest request)
        {
            var validationResult = await _validator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors
                    .GroupBy(e => e.PropertyName, e => e.ErrorMessage)
                    .ToDictionary(k => k.Key, v => v.ToArray());
                throw new ValidationException(errors);
            }

            if (!await _productRepository.IsNameUniqueAsync(request.Name))
            {
                var errors = new Dictionary<string, string[]>
                { { "Name", new[] { $"Product '{request.Name}' already exists." } } };
                throw new ValidationException(errors);
            }


            if (request.BrandId.HasValue)
            {
                var brand = await _brandRepository.GetByIdAsync(request.BrandId.Value);
                if (brand == null)
                {
                    var errors = new Dictionary<string, string[]>
                    { { "BrandId", new[] { "Selected Brand does not exist." } } };
                    throw new ValidationException(errors);
                }
            }

            foreach (var catId in request.CategoryIds)
            {
                if (!await _categoryRepository.ExistsAsync(catId))
                {
                    var errors = new Dictionary<string, string[]>
                    { { "CategoryIds", new[] { $"Category with ID '{catId}' does not exist." } } };
                    throw new ValidationException(errors);
                }
            }

            var productId = Guid.NewGuid();
            var slug = request.Name.ToLower().Trim().Replace(" ", "-");

            var product = new Domain.Entities.Product
            {
                Id = productId,
                Name = request.Name,
                Slug = slug,
                Description = request.Description,
                MainImageUrl = request.MainImageUrl,
                BrandId = request.BrandId,
                Status = request.Status,
                IsDeleted = false
            };

            foreach (var catId in request.CategoryIds)
            {
                product.ProductCategories.Add(new ProductCategory
                {
                    ProductId = productId,
                    CategoryId = catId
                });
            }

            foreach (var variantDto in request.Variants)
            {
                var variantId = Guid.NewGuid();

                var imagesJson = JsonSerializer.Serialize(variantDto.ImageUrls);

                var variant = new ProductVariants
                {
                    Id = variantId,
                    ProductId = productId,
                    Sku = variantDto.Sku,
                    Size = variantDto.Size,
                    Colour = variantDto.Colour,
                    Price = variantDto.Price,
                    Weight = variantDto.Weight,
                    InventoryQty = variantDto.InventoryQty,
                    IsActive = variantDto.IsActive,
                    IsDeleted = false
                };

                var productImage = new ProductImages
                {
                    Id = Guid.NewGuid(),
                    ProductId = productId,
                    VariantId = variantId,
                    ImagesJson = imagesJson,
                    IsDeleted = false
                };

                variant.ProductImages.Add(productImage);
                product.Variants.Add(variant);
            }

            await _productRepository.AddAsync(product);

            return product.Id;
        }
    }
}
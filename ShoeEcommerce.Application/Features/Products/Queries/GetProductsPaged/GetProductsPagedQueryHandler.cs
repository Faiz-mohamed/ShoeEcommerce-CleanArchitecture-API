using FluentValidation;
using ShoeEcommerce.Application.Common.Exceptions;
using ShoeEcommerce.Application.Features.Products.DTOs;
using ShoeEcommerce.Application.Interfaces.Repositories;

namespace ShoeEcommerce.Application.Features.Product.Queries.GetProductsPaged
{
    public class GetProductsPagedQueryHandler
    {
        private readonly IProductRepository _productRepository;
        private readonly IValidator<GetProductsPagedQuery> _validator;

        public GetProductsPagedQueryHandler(
            IProductRepository productRepository,
            IValidator<GetProductsPagedQuery> validator)
        {
            _productRepository = productRepository;
            _validator = validator;
        }

        public async Task<PagedResult<ProductDto>> HandleAsync(GetProductsPagedQuery query)
        {
            var validationResult = await _validator.ValidateAsync(query);
            if (!validationResult.IsValid)
            {
                throw new ShoeEcommerce.Application.Common.Exceptions.ValidationException(validationResult.ToDictionary());
            }

            var (products, totalCount) = await _productRepository.GetPagedAsync(
                query.PageNumber,
                query.PageSize,
                query.CategoryId,
                query.CategorySlug,
                query.SearchTerm
            );

            var dtos = products.Select(p => new ProductDto
            {
                Id = p.Id,
                Name = p.Name,
                Slug = p.Slug,
                BrandName = p.Brand?.Name ?? "",
                MainImageUrl = p.MainImageUrl,
                PriceStart = p.Variants.Any() ? p.Variants.Min(v => v.Price) : 0,
                CategoryName = p.ProductCategories.FirstOrDefault()?.Category?.Name ?? string.Empty
            }).ToList();

            return new PagedResult<ProductDto>
            {
                Items = dtos,
                TotalCount = totalCount,
                PageNumber = query.PageNumber,
                PageSize = query.PageSize
            };
        }
    }
}
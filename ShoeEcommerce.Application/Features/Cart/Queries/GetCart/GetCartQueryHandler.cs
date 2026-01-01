using ShoeEcommerce.Application.Common.Interfaces.Repositories;
using ShoeEcommerce.Application.Common.Interfaces.Services;
using ShoeEcommerce.Application.Features.Cart.DTOs;
using System.Text.Json; // Required for handling ImageUrls

namespace ShoeEcommerce.Application.Features.Cart.Queries.GetCart
{
    public class GetCartQuery { }

    public class GetCartQueryHandler
    {
        private readonly ICartRepository _cartRepository;
        private readonly ICurrentUserService _currentUserService;

        public GetCartQueryHandler(
            ICartRepository cartRepository,
            ICurrentUserService currentUserService)
        {
            _cartRepository = cartRepository;
            _currentUserService = currentUserService;
        }

        public async Task<CartDto> Handle(GetCartQuery query)
        {
            var userIdString = _currentUserService.UserId;
            if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var userId))
            {
                throw new UnauthorizedAccessException("User is not logged in.");
            }

            var cart = await _cartRepository.GetByUserIdAsync(userId);

            if (cart == null)
            {
                return new CartDto
                {
                    UserId = userId,
                    TotalPrice = 0,
                    Items = new List<CartItemDto>()
                };
            }

            return new CartDto
            {
                Id = cart.Id,
                UserId = cart.UserId,
                TotalPrice = cart.TotalPrice,
                Items = cart.Items.Select(i =>
                {
                    var variantImagesEntry = i.ProductVariant.ProductImages.FirstOrDefault();
                    var imageUrls = ParseImages(variantImagesEntry?.ImagesJson);

                    return new CartItemDto
                    {
                        Id = i.Id,
                        ProductVariantId = i.ProductVariantId,
                        ProductName = i.ProductVariant.Product.Name,

                        ImageUrls = imageUrls,

                        Size = i.ProductVariant.Size ?? "N/A",
                        Color = i.ProductVariant.Colour ?? "N/A",
                        UnitPrice = i.UnitPrice,
                        Quantity = i.Quantity
                    };
                }).ToList()
            };
        }

        private List<string> ParseImages(string? json)
        {
            if (string.IsNullOrWhiteSpace(json)) return new List<string>();
            try
            {
                return JsonSerializer.Deserialize<List<string>>(json) ?? new List<string>();
            }
            catch
            {
                return new List<string>();
            }
        }
    }
}
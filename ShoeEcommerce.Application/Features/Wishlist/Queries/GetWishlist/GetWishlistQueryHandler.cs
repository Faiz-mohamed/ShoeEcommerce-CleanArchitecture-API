using ShoeEcommerce.Application.Common.Interfaces.Repositories;
using ShoeEcommerce.Application.Common.Interfaces.Services;
using ShoeEcommerce.Application.Features.Wishlist.DTOs;

namespace ShoeEcommerce.Application.Features.Wishlist.Queries.GetWishlist
{
    public class GetWishlistQuery { }

    public class GetWishlistQueryHandler
    {
        private readonly IWishlistRepository _wishlistRepository;
        private readonly ICurrentUserService _currentUserService;

        public GetWishlistQueryHandler(
            IWishlistRepository wishlistRepository,
            ICurrentUserService currentUserService)
        {
            _wishlistRepository = wishlistRepository;
            _currentUserService = currentUserService;
        }

        public async Task<WishlistDto> Handle(GetWishlistQuery query)
        {
            var userIdString = _currentUserService.UserId;
            if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var userId))
            {
                throw new UnauthorizedAccessException();
            }

            var wishlist = await _wishlistRepository.GetByUserIdAsync(userId);

            if (wishlist == null)
            {
                return new WishlistDto { UserId = userId, Items = new List<WishlistItemDto>() };
            }

            return new WishlistDto
            {
                Id = wishlist.Id,
                UserId = wishlist.UserId,
                Items = wishlist.Items.Select(item => new WishlistItemDto
                {
                    ProductId = item.ProductId,
                    ProductName = item.Product.Name,
                    Price = item.Product.Variants.FirstOrDefault()?.Price ?? 0,
                    BrandName = item.Product.Brand?.Name ?? "Unknown",
                    MainImageUrl = item.Product.MainImageUrl ?? string.Empty
                }).ToList()
            };
        }
    }
}
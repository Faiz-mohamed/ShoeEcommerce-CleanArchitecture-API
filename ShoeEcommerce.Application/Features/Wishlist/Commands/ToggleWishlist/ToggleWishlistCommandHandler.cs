using ShoeEcommerce.Application.Common.Exceptions;
using ShoeEcommerce.Application.Common.Interfaces.Repositories;
using ShoeEcommerce.Application.Common.Interfaces.Services;
using ShoeEcommerce.Application.Features.Wishlist.DTOs;
using ShoeEcommerce.Application.Interfaces.Repositories;
using ShoeEcommerce.Domain.Entities;

namespace ShoeEcommerce.Application.Features.Wishlist.Commands.ToggleWishlist
{
    public class ToggleWishlistCommandHandler
    {
        private readonly IWishlistRepository _wishlistRepository;
        private readonly IProductRepository _productRepository;
        private readonly ICurrentUserService _currentUserService;

        public ToggleWishlistCommandHandler(
            IWishlistRepository wishlistRepository,
            IProductRepository productRepository,
            ICurrentUserService currentUserService)
        {
            _wishlistRepository = wishlistRepository;
            _productRepository = productRepository;
            _currentUserService = currentUserService;
        }

        public async Task<string> Handle(ToggleWishlistRequest request)
        {
            var userIdString = _currentUserService.UserId;
            if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var userId))
            {
                throw new UnauthorizedAccessException("User not logged in.");
            }

            var product = await _productRepository.GetByIdAsync(request.ProductId);
            if (product == null)
            {
                throw new NotFoundException($"Product with ID {request.ProductId} found.");
            }

            var wishlist = await _wishlistRepository.GetByUserIdAsync(userId);

            if (wishlist == null)
            {
                wishlist = new Domain.Entities.Wishlist
                {
                    UserId = userId,
                    Items = new List<WishlistItem>()
                };
                await _wishlistRepository.AddAsync(wishlist);
            }


            var existingItem = wishlist.Items.FirstOrDefault(i => i.ProductId == request.ProductId);

            string status;

            if (existingItem != null)
            {
                wishlist.Items.Remove(existingItem);
                status = "Removed";
            }
            else
            {
                wishlist.Items.Add(new WishlistItem
                {
                    ProductId = request.ProductId,
                    WishlistId = wishlist.Id
                });
                status = "Added";
            }

            await _wishlistRepository.SaveChangesAsync();

            return status;
        }
    }
}
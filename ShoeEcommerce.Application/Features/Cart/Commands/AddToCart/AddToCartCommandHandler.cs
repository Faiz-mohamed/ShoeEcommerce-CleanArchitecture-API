using FluentValidation;
using ShoeEcommerce.Application.Common.Exceptions;
using ShoeEcommerce.Application.Common.Interfaces.Repositories;
using ShoeEcommerce.Application.Common.Interfaces.Services;
using ShoeEcommerce.Application.Features.Cart.DTOs;
using ShoeEcommerce.Application.Interfaces.Repositories;
using ShoeEcommerce.Domain.Entities;
using ValidationException = ShoeEcommerce.Application.Common.Exceptions.ValidationException;

namespace ShoeEcommerce.Application.Features.Cart.Commands.AddToCart
{
    public class AddToCartCommandHandler
    {
        private readonly ICartRepository _cartRepository;
        private readonly IProductRepository _productRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IValidator<AddToCartRequest> _validator;

        public AddToCartCommandHandler(
            ICartRepository cartRepository,
            IProductRepository productRepository,
            ICurrentUserService currentUserService,
            IValidator<AddToCartRequest> validator)
        {
            _cartRepository = cartRepository;
            _productRepository = productRepository;
            _currentUserService = currentUserService;
            _validator = validator;
        }

        public async Task<Guid> Handle(AddToCartRequest request)
        {
            var validationResult = await _validator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors
                    .GroupBy(e => e.PropertyName, e => e.ErrorMessage)
                    .ToDictionary(k => k.Key, v => v.ToArray());
                throw new ValidationException(errors);
            }

            var userIdString = _currentUserService.UserId;
            if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var userId))
            {
                throw new UnauthorizedAccessException("Invalid User ID.");
            }

            var product = await _productRepository.GetByVariantIdAsync(request.ProductVariantId);
            var variant = product?.Variants.FirstOrDefault(v => v.Id == request.ProductVariantId);

            if (product == null || variant == null)
            {
                throw new NotFoundException("ProductVariant", request.ProductVariantId);
            }

            if (variant.InventoryQty < request.Quantity)
            {
                throw new Exception($"Insufficient stock. Only {variant.InventoryQty} items left.");
            }

            var cart = await _cartRepository.GetByUserIdAsync(userId);
            if (cart == null)
            {
                cart = new Domain.Entities.Cart
                {
                    UserId = userId,
                    Items = new List<CartItem>()
                };
                await _cartRepository.AddAsync(cart);
            }


            var existingItem = cart.Items.FirstOrDefault(i => i.ProductVariantId == request.ProductVariantId);

            if (existingItem != null)
            {

                if (variant.InventoryQty < (existingItem.Quantity + request.Quantity))
                {
                    throw new Exception($"Cannot add more. You already have {existingItem.Quantity} in cart, and we only have {variant.InventoryQty} in stock.");
                }


                existingItem.Quantity += request.Quantity;

                existingItem.UnitPrice = variant.Price;
            }
            else
            {
                var newItem = new CartItem
                {
                    ProductVariantId = request.ProductVariantId,
                    Quantity = request.Quantity,
                    UnitPrice = variant.Price,
                    CartId = cart.Id
                };
                cart.Items.Add(newItem);
            }

            await _cartRepository.SaveChangesAsync();

            return cart.Id;
        }
    }
}
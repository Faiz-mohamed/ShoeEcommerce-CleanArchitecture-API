using FluentValidation;
using ShoeEcommerce.Application.Common.Exceptions;
using ShoeEcommerce.Application.Common.Helpers;
using ShoeEcommerce.Application.Common.Interfaces.Repositories;
using ShoeEcommerce.Application.Common.Interfaces.Services;
using ShoeEcommerce.Application.Features.Orders.DTOs;
using ShoeEcommerce.Domain.Entities;
using ShoeEcommerce.Domain.Enums;
using ValidationException = ShoeEcommerce.Application.Common.Exceptions.ValidationException;

namespace ShoeEcommerce.Application.Features.Orders.Commands.CreateOrder
{
    public class CreateOrderCommandHandler
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly ICartRepository _cartRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IPaymentService _paymentService;
        private readonly IValidator<CreateOrderRequest> _validator;

        public CreateOrderCommandHandler(
            ICurrentUserService currentUserService,
            ICartRepository cartRepository,
            IOrderRepository orderRepository,
            IPaymentService paymentService,
            IValidator<CreateOrderRequest> validator)
        {
            _currentUserService = currentUserService;
            _cartRepository = cartRepository;
            _orderRepository = orderRepository;
            _paymentService = paymentService;
            _validator = validator;
        }

        public async Task<string> Handle(CreateOrderRequest request)
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
                throw new System.UnauthorizedAccessException("User is not logged in.");
            }

            var cart = await _cartRepository.GetByUserIdAsync(userId);
            if (cart == null || !cart.Items.Any())
            {
                throw new ConflictException("Cart is empty. Cannot create order.");
            }

            decimal totalAmount = 0;
            foreach (var item in cart.Items)
            {
                if (item.ProductVariant.InventoryQty < item.Quantity)
                {
                    throw new ConflictException($"Product '{item.ProductVariant.Product.Name}' (Size: {item.ProductVariant.Size}) is out of stock.");
                }
                totalAmount += (item.ProductVariant.Price * item.Quantity);
            }

            var orderId = Guid.NewGuid();

            var razorpayOrderId = await _paymentService.CreateOrderAsync(totalAmount, "INR", orderId.ToString());

            var order = new Order
            {
                Id = orderId,
                UserId = userId,
                TotalAmount = totalAmount,
                Status = OrderStatus.Pending,
                RazorpayOrderId = razorpayOrderId,

                // Address Data
                ShippingAddress = request.ShippingAddress,
                City = request.City,
                PostalCode = request.PostalCode,
                Country = request.Country,

                PhoneNumber = PhoneNumberHelper.Normalize(request.PhoneNumber, request.Country) ?? request.PhoneNumber,

                CreatedAt = DateTime.UtcNow,

                OrderItems = cart.Items.Select(ci => new OrderItem
                {
                    Id = Guid.NewGuid(),
                    OrderId = orderId,
                    ProductVariantId = ci.ProductVariantId,
                    ProductName = ci.ProductVariant.Product.Name,
                    ProductSku = ci.ProductVariant.Sku,
                    Size = ci.ProductVariant.Size ?? "N/A",
                    Color = ci.ProductVariant.Colour ?? "N/A",
                    UnitPrice = ci.ProductVariant.Price,
                    Quantity = ci.Quantity
                }).ToList()
            };

            await _orderRepository.AddAsync(order);

            return razorpayOrderId;
        }
    }
}
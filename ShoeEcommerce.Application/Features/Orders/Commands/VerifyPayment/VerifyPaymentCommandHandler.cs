using FluentValidation;
using ShoeEcommerce.Application.Common.Exceptions;
using ShoeEcommerce.Application.Common.Interfaces.Repositories;
using ShoeEcommerce.Application.Common.Interfaces.Services;
using ShoeEcommerce.Application.Features.Orders.DTOs;
using ShoeEcommerce.Domain.Enums;
using ValidationException = ShoeEcommerce.Application.Common.Exceptions.ValidationException;

namespace ShoeEcommerce.Application.Features.Orders.Commands.VerifyPayment
{
    public class VerifyPaymentCommandHandler
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IPaymentService _paymentService;
        private readonly ICartRepository _cartRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IValidator<VerifyPaymentRequest> _validator;

        public VerifyPaymentCommandHandler(
            IOrderRepository orderRepository,
            IPaymentService paymentService,
            ICartRepository cartRepository,
            ICurrentUserService currentUserService,
            IValidator<VerifyPaymentRequest> validator)
        {
            _orderRepository = orderRepository;
            _paymentService = paymentService;
            _cartRepository = cartRepository;
            _currentUserService = currentUserService;
            _validator = validator;
        }

        public async Task<bool> Handle(VerifyPaymentRequest request)
        {
            var validationResult = await _validator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors
                    .GroupBy(e => e.PropertyName, e => e.ErrorMessage)
                    .ToDictionary(k => k.Key, v => v.ToArray());
                throw new ValidationException(errors);
            }

            var isValid = _paymentService.VerifyPaymentSignature(
                request.RazorpayOrderId,
                request.RazorpayPaymentId,
                request.RazorpaySignature);

            if (!isValid)
            {
                throw new BadRequestException("Invalid Payment Signature.");
            }

            var order = await _orderRepository.GetByRazorpayOrderIdAsync(request.RazorpayOrderId);
            if (order == null) throw new NotFoundException("Order not found.");

            if (order.Status == OrderStatus.Paid)
            {
                return true;
            }

            order.RazorpayPaymentId = request.RazorpayPaymentId;
            order.RazorpaySignature = request.RazorpaySignature;
            order.Status = OrderStatus.Paid;

            foreach (var item in order.OrderItems)
            {
                if (item.ProductVariant != null)
                {
                    if (item.ProductVariant.InventoryQty < item.Quantity)
                    {
                        // EDGE CASE: Race Condition (Stock ran out while user was typing card details)
                        // In Production: We would mark order as "PaymentFailed" and auto-refund.
                        order.Status = OrderStatus.PaymentFailed;
                        await _orderRepository.UpdateAsync(order);
                        throw new ConflictException($"Product {item.ProductName} is out of stock. Payment will be refunded.");
                    }

                    item.ProductVariant.InventoryQty -= item.Quantity;
                }
            }

            await _cartRepository.ClearCartAsync(order.UserId);

            await _orderRepository.UpdateAsync(order);

            return true;
        }
    }
}
using FluentValidation;
using ShoeEcommerce.Application.Features.Orders.DTOs;

namespace ShoeEcommerce.Application.Features.Orders.Validators
{
    public class VerifyPaymentValidator : AbstractValidator<VerifyPaymentRequest>
    {
        public VerifyPaymentValidator()
        {
            RuleFor(x => x.RazorpayOrderId).NotEmpty().WithMessage("Order ID is missing.");
            RuleFor(x => x.RazorpayPaymentId).NotEmpty().WithMessage("Payment ID is missing.");
            RuleFor(x => x.RazorpaySignature).NotEmpty().WithMessage("Signature is missing.");
        }
    }
}
using FluentValidation;
using ShoeEcommerce.Application.Common.Helpers;
using ShoeEcommerce.Application.Features.Orders.DTOs;

namespace ShoeEcommerce.Application.Features.Orders.Validators
{
    public class CreateOrderValidator : AbstractValidator<CreateOrderRequest>
    {
        public CreateOrderValidator()
        {
            RuleFor(x => x.ShippingAddress)
                .NotEmpty().WithMessage("Shipping address is required.")
                .MaximumLength(200).WithMessage("Address cannot exceed 200 characters.");

            RuleFor(x => x.City)
                .NotEmpty().WithMessage("City is required.")
                .MaximumLength(100);

            RuleFor(x => x.Country)
                .NotEmpty().WithMessage("Country is required.");

            RuleFor(x => x.PostalCode)
                .NotEmpty().WithMessage("Postal Code is required.")
                .MaximumLength(20);

            RuleFor(x => x.PhoneNumber)
                .NotEmpty().WithMessage("Phone number is required.")
                .Must((request, phone) =>
                {
                    return PhoneNumberHelper.IsValid(phone, request.Country);
                })
                .WithMessage("Invalid phone number format for the specified country.");
        }
    }
}
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShoeEcommerce.Application.Features.Orders.Commands.CreateOrder;
using ShoeEcommerce.Application.Features.Orders.Commands.VerifyPayment;
using ShoeEcommerce.Application.Features.Orders.DTOs;

namespace ShoeEcommerce.API.Controllers
{
    [Authorize]
    [Route("api/orders")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly CreateOrderCommandHandler _createOrderHandler;
        private readonly VerifyPaymentCommandHandler _verifyPaymentHandler;

        public OrdersController(
            CreateOrderCommandHandler createOrderHandler,
            VerifyPaymentCommandHandler verifyPaymentHandler)
        {
            _createOrderHandler = createOrderHandler;
            _verifyPaymentHandler = verifyPaymentHandler;
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequest request)
        {
            var razorpayOrderId = await _createOrderHandler.Handle(request);

            return Ok(new
            {
                RazorpayOrderId = razorpayOrderId,
                Message = "Order created. Proceed to payment."
            });
        }

        [HttpPost("verify")]
        public async Task<IActionResult> VerifyPayment([FromBody] VerifyPaymentRequest request)
        {
            await _verifyPaymentHandler.Handle(request);

            return Ok(new { Message = "Payment verified. Order placed successfully!" });
        }
    }
}
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShoeEcommerce.Application.Features.Cart.Commands.AddToCart;
using ShoeEcommerce.Application.Features.Cart.DTOs;
using ShoeEcommerce.Application.Features.Cart.Queries.GetCart;

namespace ShoeEcommerce.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class CartController : ControllerBase
    {
        private readonly AddToCartCommandHandler _addToCartHandler;
        private readonly GetCartQueryHandler _getCartHandler;

        public CartController(
            AddToCartCommandHandler addToCartHandler,
            GetCartQueryHandler getCartHandler)
        {
            _addToCartHandler = addToCartHandler;
            _getCartHandler = getCartHandler;
        }

        [HttpPost("items")]
        public async Task<IActionResult> AddToCart([FromBody] AddToCartRequest request)
        {
            var result = await _addToCartHandler.Handle(request);
            return Ok(new { Message = "Item added to cart", CartId = result });
        }

        [HttpGet]
        public async Task<IActionResult> GetMyCart()
        {
            var result = await _getCartHandler.Handle(new GetCartQuery());
            return Ok(result);
        }
    }
}
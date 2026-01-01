using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShoeEcommerce.Application.Features.Wishlist.Commands.ToggleWishlist;
using ShoeEcommerce.Application.Features.Wishlist.DTOs;
using ShoeEcommerce.Application.Features.Wishlist.Queries.GetWishlist;

namespace ShoeEcommerce.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class WishlistController : ControllerBase
    {
        private readonly ToggleWishlistCommandHandler _toggleHandler;
        private readonly GetWishlistQueryHandler _getWishlistHandler;

        public WishlistController(
            ToggleWishlistCommandHandler toggleHandler,
            GetWishlistQueryHandler getWishlistHandler)
        {
            _toggleHandler = toggleHandler;
            _getWishlistHandler = getWishlistHandler;
        }

        [HttpPost("toggle")]
        public async Task<IActionResult> ToggleWishlist([FromBody] ToggleWishlistRequest request)
        {
            var status = await _toggleHandler.Handle(request);

            return Ok(new
            {
                Message = $"Product {status} wishlist",
                Status = status
            });
        }

        [HttpGet]
        public async Task<IActionResult> GetMyWishlist()
        {
            var result = await _getWishlistHandler.Handle(new GetWishlistQuery());
            return Ok(result);
        }
    }
}
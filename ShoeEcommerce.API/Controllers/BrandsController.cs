using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShoeEcommerce.Application.Features.Brands.Commands.CreateBrand;
using ShoeEcommerce.Application.Features.Brands.DTOs;

namespace ShoeEcommerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BrandsController : ControllerBase
    {
        private readonly CreateBrandCommandHandler _createBrandHandler;

        public BrandsController(CreateBrandCommandHandler createBrandHandler)
        {
            _createBrandHandler = createBrandHandler;
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Create([FromBody] CreateBrandRequest request)
        {
            var brandId = await _createBrandHandler.Handle(request);

            return StatusCode(201, new { Id = brandId, Message = "Brand created successfully" });
        }
    }
}
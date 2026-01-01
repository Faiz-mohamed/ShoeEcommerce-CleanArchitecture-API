using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShoeEcommerce.Application.Features.Categories.Commands.CreateCategory;
using ShoeEcommerce.Application.Features.Categories.DTOs;

namespace ShoeEcommerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly CreateCategoryCommandHandler _createCategoryHandler;

        public CategoriesController(CreateCategoryCommandHandler createCategoryHandler)
        {
            _createCategoryHandler = createCategoryHandler;
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Create([FromBody] CreateCategoryRequest request)
        {
            var categoryId = await _createCategoryHandler.Handle(request);

            return StatusCode(201, new { Id = categoryId, Message = "Category created successfully" });
        }
    }
}
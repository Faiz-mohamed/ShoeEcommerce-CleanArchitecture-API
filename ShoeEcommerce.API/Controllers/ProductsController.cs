using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShoeEcommerce.Application.Features.Product.Queries.GetProductById;
using ShoeEcommerce.Application.Features.Product.Queries.GetProductByVariant;
using ShoeEcommerce.Application.Features.Product.Queries.GetProductsPaged;
using ShoeEcommerce.Application.Features.Products.Commands.CreateProduct;
using ShoeEcommerce.Application.Features.Products.DTOs;

namespace ShoeEcommerce.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly GetProductsPagedQueryHandler _pagedHandler;
        private readonly GetProductByIdQueryHandler _byIdHandler;
        private readonly GetProductByVariantQueryHandler _byVariantHandler;

        private readonly CreateProductCommandHandler _createHandler;

        public ProductsController(
            GetProductsPagedQueryHandler pagedHandler,
            GetProductByIdQueryHandler byIdHandler,
            GetProductByVariantQueryHandler byVariantHandler,
            CreateProductCommandHandler createHandler)
        {
            _pagedHandler = pagedHandler;
            _byIdHandler = byIdHandler;
            _byVariantHandler = byVariantHandler;
            _createHandler = createHandler;
        }

        // =================================================================
        // WRITE OPERATIONS (Commands)
        // =================================================================

        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Create([FromBody] CreateProductRequest request)
        {
            var productId = await _createHandler.Handle(request);

            return StatusCode(201, new { Id = productId, Message = "Product created successfully" });
        }

        // =================================================================
        // READ OPERATIONS (Queries)
        // =================================================================

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] GetProductsPagedQuery query)
        {
            var result = await _pagedHandler.HandleAsync(query);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var query = new GetProductByIdQuery(id);
            var result = await _byIdHandler.HandleAsync(query);
            return Ok(result);
        }

        [HttpGet("variant/{variantId}")]
        public async Task<IActionResult> GetByVariantId(Guid variantId)
        {
            var query = new GetProductByVariantQuery(variantId);
            var result = await _byVariantHandler.HandleAsync(query);
            return Ok(result);
        }
    }
}
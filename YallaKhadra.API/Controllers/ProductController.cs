using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using YallaKhadra.API.Bases;
using YallaKhadra.Core.Bases.Responses;
using YallaKhadra.Core.Enums;
using YallaKhadra.Core.Features.Products.Commands.RequestModels;
using YallaKhadra.Core.Features.Products.Commands.Responses;
using YallaKhadra.Core.Features.Products.Queries.Models;
using YallaKhadra.Core.Features.Products.Queries.Responses;

namespace YallaKhadra.API.Controllers {
    /// <summary>
    /// Product management controller for e-commerce operations
    /// </summary>
    public class ProductController : BaseController {

        /// <summary>
        /// Add a new product with images
        /// </summary>
        /// <param name="command">Product details including name, description, points cost, stock, and exactly 3 images (1 main + 2 additional)</param>
        /// <returns>Created product with images</returns>
        /// <response code="201">Product created successfully</response>
        /// <response code="400">Invalid input data or product creation failed</response>
        /// <response code="401">User is not authenticated</response>
        [HttpPost("add")]
        [Authorize(Roles = $"{nameof(UserRole.SuperAdmin)},{nameof(UserRole.Admin)}")]
        [ProducesResponseType(typeof(Response<AddProductResponse>), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> AddProduct([FromForm] AddProductCommand command) {
            var result = await Mediator.Send(command);
            return NewResult(result);
        }

        /// <summary>
        /// Get all products with pagination
        /// </summary>
        /// <param name="query">Pagination parameters and optional category filter</param>
        /// <returns>Paginated list of products with their details</returns>
        /// <response code="200">Returns paginated list of products</response>
        /// <response code="400">Invalid pagination parameters</response>
        [HttpGet]
        [ProducesResponseType(typeof(PaginatedResult<GetProductsPaginatedResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetAll([FromQuery] GetProductsPaginatedQuery query) {
            var result = await Mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// Get product by its unique identifier
        /// </summary>
        /// <param name="query">Product ID (int)</param>
        /// <returns>Product details including name, description, points cost, stock, and all images</returns>
        /// <response code="200">Returns product details</response>
        /// <response code="404">Product not found</response>
        /// <response code="400">Invalid product ID format</response>
        [HttpGet("id/{Id:int}")]
        [ProducesResponseType(typeof(Response<GetProductByIdResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetById([FromRoute] GetProductByIdQuery query) {
            var result = await Mediator.Send(query);
            return NewResult(result);
        }

        /// <summary>
        /// Update an existing product
        /// </summary>
        /// <param name="command">Product update details (all fields are optional except ID). If images are provided, exactly 3 images must be uploaded.</param>
        /// <returns>Updated product with images</returns>
        /// <response code="200">Product updated successfully</response>
        /// <response code="400">Invalid input data or product update failed</response>
        /// <response code="404">Product not found</response>
        /// <response code="401">User is not authenticated</response>
        [HttpPatch("{Id}")]
        [Authorize(Roles = $"{nameof(UserRole.SuperAdmin)},{nameof(UserRole.Admin)}")]
        [ProducesResponseType(typeof(Response<UpdateProductResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> UpdateProduct([FromRoute] int Id, [FromForm] UpdateProductCommand command) {
            command.Id = Id;
            var result = await Mediator.Send(command);
            return NewResult(result);
        }

        /// <summary>
        /// Delete a product and all its associated images
        /// </summary>
        /// <param name="id">Product ID to delete</param>
        /// <returns>Confirmation of deleted product</returns>
        /// <response code="200">Product deleted successfully</response>
        /// <response code="404">Product not found</response>
        /// <response code="400">Delete operation failed</response>
        /// <response code="401">User is not authenticated</response>
        [HttpDelete("delete/{id:int}")]
        [Authorize(Roles = $"{nameof(UserRole.SuperAdmin)},{nameof(UserRole.Admin)}")]
        [ProducesResponseType(typeof(Response), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> DeleteProduct(int id) {
            var command = new DeleteProductCommand { Id = id };
            var result = await Mediator.Send(command);
            return NewResult(result);
        }
    }
}

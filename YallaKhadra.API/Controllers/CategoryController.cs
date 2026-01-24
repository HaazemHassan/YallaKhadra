using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using YallaKhadra.API.Bases;
using YallaKhadra.Core.Bases.Responses;
using YallaKhadra.Core.Enums;
using YallaKhadra.Core.Features.Categories.Commands.RequestModels;
using YallaKhadra.Core.Features.Categories.Commands.Responses;
using YallaKhadra.Core.Features.Categories.Queries.Models;
using YallaKhadra.Core.Features.Categories.Queries.Responses;

namespace YallaKhadra.API.Controllers {
    /// <summary>
    /// Category management controller for e-commerce operations
    /// </summary>
    [ApiController]
    [Produces("application/json")]
    public class CategoryController : BaseController {

        /// <summary>
        /// Get category by its unique identifier
        /// </summary>
        /// <param name="id">Category ID</param>
        /// <returns>Category details including name and description</returns>
        /// <response code="200">Returns category details</response>
        /// <response code="404">Category not found</response>
        /// <response code="400">Invalid category ID format</response>
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(Response<GetCategoryByIdResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetById([FromRoute] int id) {
            var query = new GetCategoryByIdQuery { Id = id };
            var result = await Mediator.Send(query);
            return NewResult(result);
        }

        /// <summary>
        /// Get category by name
        /// </summary>
        /// <param name="name">Category name</param>
        /// <returns>Category details including id, name and description</returns>
        /// <response code="200">Returns category details</response>
        /// <response code="404">Category not found</response>
        /// <response code="400">Invalid request</response>
        [HttpGet("name/{name}")]
        [ProducesResponseType(typeof(Response<GetCategoryByNameResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetByName([FromRoute] string name) {
            var query = new GetCategoryByNameQuery { Name = name };
            var result = await Mediator.Send(query);
            return NewResult(result);
        }



        /// <summary>
        /// Add a new category (Admin only)
        /// </summary>
        /// <param name="command">Category details including name and description</param>
        /// <returns>Created category details</returns>
        /// <response code="201">Category created successfully</response>
        /// <response code="400">Invalid input data</response>
        /// <response code="409">Category with the same name already exists</response>
        /// <response code="401">User is not authenticated</response>
        [HttpPost]
        [Authorize(Roles = $"{nameof(UserRole.SuperAdmin)},{nameof(UserRole.Admin)}")]
        [ProducesResponseType(typeof(Response<AddCategoryResponse>), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> AddCategory([FromBody] AddCategoryCommand command) {
            var result = await Mediator.Send(command);
            return NewResult(result);
        }

        /// <summary>
        /// Partially update an existing category (Admin only)
        /// </summary>
        /// <param name="id">Category ID</param>
        /// <param name="command">Updated category details (Name and/or Description)</param>
        /// <returns>Updated category details</returns>
        /// <response code="200">Category updated successfully</response>
        /// <response code="404">Category not found</response>
        /// <response code="400">Invalid input data or no fields provided for update</response>
        /// <response code="409">Another category with the same name already exists</response>
        /// <response code="401">User is not authenticated</response>
        [HttpPatch("{id:int}")]
        [Authorize(Roles = $"{nameof(UserRole.SuperAdmin)},{nameof(UserRole.Admin)}")]
        [ProducesResponseType(typeof(Response<UpdateCategoryResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> UpdateCategory([FromRoute] int id, [FromBody] UpdateCategoryCommand command) {
            command.Id = id;
            var result = await Mediator.Send(command);
            return NewResult(result);
        }

        /// <summary>
        /// Delete a category (Admin only)
        /// </summary>
        /// <param name="id">Category ID</param>
        /// <returns>Success message</returns>
        /// <response code="200">Category deleted successfully</response>
        /// <response code="404">Category not found</response>
        /// <response code="400">Cannot delete category that has products</response>
        /// <response code="401">User is not authenticated</response>
        [HttpDelete("{id:int}")]
        [Authorize(Roles = $"{nameof(UserRole.SuperAdmin)},{nameof(UserRole.Admin)}")]
        [ProducesResponseType(typeof(Response), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> DeleteCategory([FromRoute] int id) {
            var command = new DeleteCategoryCommand { Id = id };
            var result = await Mediator.Send(command);
            return NewResult(result);
        }
    }
}

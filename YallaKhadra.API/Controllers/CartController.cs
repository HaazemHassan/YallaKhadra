using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using YallaKhadra.API.Bases;
using YallaKhadra.Core.Bases.Responses;
using YallaKhadra.Core.Enums;
using YallaKhadra.Core.Features.Carts.Commands.RequestModels;
using YallaKhadra.Core.Features.Carts.Commands.Responses;
using YallaKhadra.Core.Features.Carts.Queries.Models;
using YallaKhadra.Core.Features.Carts.Queries.Responses;

namespace YallaKhadra.API.Controllers {
    /// <summary>
    /// Shopping cart controller for managing user cart operations
    /// </summary>
    [Authorize(Roles = nameof(UserRole.User))]
    public class CartController : BaseController {

        /// <summary>
        /// Get current user's shopping cart with all items
        /// </summary>
        /// <returns>Cart details including items, total points, and product information</returns>
        /// <response code="200">Returns the user's cart</response>
        /// <response code="404">Cart not found</response>
        /// <response code="401">User is not authenticated</response>
        [HttpGet]
        [ProducesResponseType(typeof(Response<GetCartResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetCart() {
            var query = new GetCartQuery();
            var result = await Mediator.Send(query);
            return NewResult(result);
        }

        /// <summary>
        /// Add a product to the shopping cart or update quantity if already exists
        /// </summary>
        /// <param name="command">Product ID and quantity to add</param>
        /// <returns>Added/updated cart item details</returns>
        /// <response code="201">Product added to cart successfully</response>
        /// <response code="200">Product quantity updated in cart</response>
        /// <response code="400">Invalid request or insufficient stock</response>
        /// <response code="404">Product not found</response>
        /// <response code="401">User is not authenticated</response>
        [HttpPost("items")]
        [ProducesResponseType(typeof(Response<AddToCartResponse>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(Response<AddToCartResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> AddToCart([FromBody] AddToCartCommand command) {
            var result = await Mediator.Send(command);
            return NewResult(result);
        }

        /// <summary>
        /// Update cart item quantity
        /// </summary>
        /// <param name="id">Cart item ID</param>
        /// <param name="command">Updated quantity</param>
        /// <returns>Updated cart item details</returns>
        /// <response code="200">Cart item quantity updated successfully</response>
        /// <response code="400">Invalid request or insufficient stock</response>
        /// <response code="404">Cart item not found</response>
        /// <response code="401">User is not authenticated</response>
        [HttpPatch("items/{id}/quantity")]
        [ProducesResponseType(typeof(Response<UpdateCartItemQuantityResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> UpdateCartItemQuantity(int id, [FromBody] UpdateCartItemQuantityCommand command) {
            command.CartItemId = id;
            var result = await Mediator.Send(command);
            return NewResult(result);
        }

        /// <summary>
        /// Toggle cart item selection status
        /// </summary>
        /// <param name="id">Cart item ID</param>
        /// <param name="command">Selection status</param>
        /// <returns>Updated cart item selection status</returns>
        /// <response code="200">Cart item selection updated successfully</response>
        /// <response code="404">Cart item not found</response>
        /// <response code="401">User is not authenticated</response>
        [HttpPatch("items/{id}/selection")]
        [ProducesResponseType(typeof(Response<ToggleCartItemSelectionResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> ToggleCartItemSelection(int id, [FromBody] ToggleCartItemSelectionCommand command) {
            command.CartItemId = id;
            var result = await Mediator.Send(command);
            return NewResult(result);
        }

        /// <summary>
        /// Remove a product from the shopping cart
        /// </summary>
        /// <param name="id">Cart item ID to remove</param>
        /// <returns>Success message</returns>
        /// <response code="200">Item removed successfully</response>
        /// <response code="404">Cart item not found</response>
        /// <response code="401">User is not authenticated</response>
        [HttpDelete("items/{id}")]
        [ProducesResponseType(typeof(Response), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> RemoveFromCart(int id) {
            var command = new RemoveFromCartCommand { CartItemId = id };
            var result = await Mediator.Send(command);
            return NewResult(result);
        }

        /// <summary>
        /// Clear all items from the shopping cart
        /// </summary>
        /// <returns>Success message</returns>
        /// <response code="200">Cart cleared successfully</response>
        /// <response code="404">Cart not found</response>
        /// <response code="401">User is not authenticated</response>
        [HttpDelete("clear")]
        [ProducesResponseType(typeof(Response), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> ClearCart() {
            var command = new ClearCartCommand();
            var result = await Mediator.Send(command);
            return NewResult(result);
        }

        /// <summary>
        /// Sync cart items prices with current product prices
        /// </summary>
        /// <returns>Success message with count of updated items</returns>
        /// <response code="200">Prices synced successfully</response>
        /// <response code="404">Cart not found</response>
        /// <response code="401">User is not authenticated</response>
        [HttpPost("sync-prices")]
        [ProducesResponseType(typeof(Response), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> SyncCartPrices() {
            var command = new SyncCartPricesCommand();
            var result = await Mediator.Send(command);
            return NewResult(result);
        }
    }
}

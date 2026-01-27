using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using YallaKhadra.API.Bases;
using YallaKhadra.Core.Bases.Responses;
using YallaKhadra.Core.Features.Orders.Commands.RequestModels;
using YallaKhadra.Core.Features.Orders.Commands.Responses;
using YallaKhadra.Core.Features.Orders.Queries.Models;
using YallaKhadra.Core.Features.Orders.Queries.Responses;

namespace YallaKhadra.API.Controllers {
    /// <summary>
    /// Orders controller for managing user orders and checkout
    /// </summary>
    [ApiController]
    [Authorize]
    public class OrderController : BaseController {

        /// <summary>
        /// Checkout and create a new order from selected cart items
        /// </summary>
        /// <param name="command">Shipping details for the order</param>
        /// <returns>Created order details including order ID and status</returns>
        /// <response code="201">Order created successfully</response>
        /// <response code="400">Invalid request or insufficient stock/points</response>
        /// <response code="404">Cart is empty or no items selected</response>
        /// <response code="401">User is not authenticated</response>
        [HttpPost("checkout")]
        [ProducesResponseType(typeof(Response<PlaceOrderResponse>), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Checkout([FromBody] PlaceOrderCommand command) {
            var result = await Mediator.Send(command);
            return NewResult(result);
        }

        /// <summary>
        /// Get current user's order history with pagination
        /// </summary>
        /// <param name="query">Pagination parameters (page number and page size)</param>
        /// <returns>Paginated list of user's orders</returns>
        /// <response code="200">Returns the user's paginated order history</response>
        /// <response code="400">Invalid pagination parameters</response>
        /// <response code="401">User is not authenticated</response>
        [HttpGet]
        [ProducesResponseType(typeof(PaginatedResult<GetMyOrdersResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetMyOrders([FromQuery] GetMyOrdersQuery query) {
            var result = await Mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// Get details of a specific order by ID
        /// </summary>
        /// <param name="id">Order ID</param>
        /// <returns>Detailed order information including items and shipping details</returns>
        /// <response code="200">Returns the order details</response>
        /// <response code="404">Order not found or does not belong to current user</response>
        /// <response code="401">User is not authenticated</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(Response<GetOrderDetailsResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetOrderById(int id) {
            var query = new GetOrderByIdQuery { Id = id };
            var result = await Mediator.Send(query);
            return NewResult(result);
        }

        /// <summary>
        /// Cancel an existing order
        /// </summary>
        /// <param name="id">Order ID to cancel</param>
        /// <returns>Confirmation message with refunded points</returns>
        /// <response code="200">Order canceled successfully and points refunded</response>
        /// <response code="400">Order cannot be canceled (already shipped/delivered/canceled)</response>
        /// <response code="404">Order not found or does not belong to current user</response>
        /// <response code="401">User is not authenticated</response>
        [HttpPatch("{id}/cancel")]
        [ProducesResponseType(typeof(Response<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> CancelOrder(int id) {
            var command = new CancelOrderCommand { OrderId = id };
            var result = await Mediator.Send(command);
            return NewResult(result);
        }
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using YallaKhadra.API.Bases;
using YallaKhadra.Core.Bases.Responses;
using YallaKhadra.Core.Enums;
using YallaKhadra.Core.Features.Orders.Commands.RequestModels;
using YallaKhadra.Core.Features.Orders.Queries.Models;
using YallaKhadra.Core.Features.Orders.Queries.Responses;

namespace YallaKhadra.API.Controllers
{
    [Authorize(Roles = $"{nameof(UserRole.SuperAdmin)},{nameof(UserRole.Admin)}")]
    [Route("api/admin/order")]
    public class AdminOrderController : BaseController
    {
        [HttpGet]
        [ProducesResponseType(typeof(PaginatedResult<GetAllOrdersForAdminsResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetAllOrders([FromQuery] GetAllOrdersForAdminsQuery query)
        {
            var result = await Mediator.Send(query);
            return Ok(result);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(Response<GetOrderDetailsResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetOrderById(int id)
        {
            var query = new GetOrderDetailsForAdminsQuery { Id = id };
            var result = await Mediator.Send(query);
            return NewResult(result);
        }

        [HttpPatch("{id}/status/next")]
        [ProducesResponseType(typeof(Response<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> AdvanceOrderStatus(int id)
        {
            var command = new AdvanceOrderStatusCommand { OrderId = id };
            var result = await Mediator.Send(command);
            return NewResult(result);
        }

        [HttpPatch("{id}/cancel")]
        [ProducesResponseType(typeof(Response<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> CancelOrder(int id)
        {
            var command = new CancelOrderCommand { OrderId = id };
            var result = await Mediator.Send(command);
            return NewResult(result);
        }
    }
}

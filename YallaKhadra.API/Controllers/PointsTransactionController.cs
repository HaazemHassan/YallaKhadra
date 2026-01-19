using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using YallaKhadra.API.Bases;
using YallaKhadra.Core.Bases.Responses;
using YallaKhadra.Core.Features.PointsTransactions.Queries.Models;
using YallaKhadra.Core.Features.PointsTransactions.Queries.Responses;

namespace YallaKhadra.API.Controllers {
    /// <summary>
    /// Points Transaction controller for managing user points and transaction history
    /// </summary>
    [ApiController]
    [Authorize]
    [Produces("application/json")]
    public class PointsTransactionController : BaseController {

        /// <summary>
        /// Get current user's points transaction history with pagination
        /// </summary>
        /// <param name="query">Pagination parameters (page number and page size)</param>
        /// <returns>Paginated list of transactions for the current user, ordered by date (newest first)</returns>
        /// <response code="200">Returns the user's paginated transaction history</response>
        /// <response code="400">Invalid pagination parameters</response>
        /// <response code="401">User is not authenticated</response>
        [HttpGet("my-transactions")]
        [ProducesResponseType(typeof(PaginatedResult<PointsTransactionResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetMyTransactions([FromQuery] GetMyTransactionsQuery query) {
            var result = await Mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// Get current user's points balance with statistics
        /// </summary>
        /// <returns>Current points balance, total earned, total spent, and transaction count</returns>
        /// <response code="200">Returns the user's balance and statistics</response>
        /// <response code="401">User is not authenticated</response>
        [HttpGet("my-balance")]
        [ProducesResponseType(typeof(Response<UserBalanceResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetMyBalance() {
            var query = new GetMyBalanceQuery();
            var result = await Mediator.Send(query);
            return NewResult(result);
        }
    }
}

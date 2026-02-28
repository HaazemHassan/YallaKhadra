using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using YallaKhadra.API.Bases;
using YallaKhadra.Core.Bases.Responses;
using YallaKhadra.Core.Enums;
using YallaKhadra.Core.Features.Leaderboard.Queries.Models;
using YallaKhadra.Core.Features.Leaderboard.Queries.Responses;

namespace YallaKhadra.API.Controllers
{
    /// <summary>
    /// Leaderboard controller for ranking users by successful waste reports
    /// </summary>
    public class LeaderboardController : BaseController
    {
        /// <summary>
        /// Get paginated leaderboard of users ranked by completed waste reports
        /// </summary>
        /// <param name="pageNumber">Page number (default: 1)</param>
        /// <param name="pageSize">Number of items per page (default: 10)</param>
        /// <param name="period">Time period filter: AllTime (0), Weekly (1), Monthly (2), Yearly (3). Default is AllTime</param>
        /// <returns>Paginated list of users with their ranks and report counts</returns>
        /// <response code="200">Leaderboard retrieved successfully</response>
        [HttpGet]
        [AllowAnonymous]
        [ProducesResponseType(typeof(PaginatedResult<LeaderboardEntryResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetLeaderboard([FromQuery] int? pageNumber,[FromQuery] int? pageSize,[FromQuery] LeaderboardPeriod period = LeaderboardPeriod.AllTime)
        {
            var query = new GetLeaderboardQuery
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                Period = period
            };

            var result = await Mediator.Send(query);
            return Ok(result);
        }
    }
}

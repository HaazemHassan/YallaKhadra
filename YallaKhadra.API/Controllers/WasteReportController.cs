using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using YallaKhadra.API.Bases;
using YallaKhadra.Core.Bases.Responses;
using YallaKhadra.Core.Features.WasteReports.Commands.RequestModels;
using YallaKhadra.Core.Features.WasteReports.Queries.Models;
using YallaKhadra.Core.Features.WasteReports.Queries.Responses;

namespace YallaKhadra.API.Controllers {
    /// <summary>
    /// Waste Report management controller for handling waste report operations
    /// </summary>
    [ApiController]
    [Authorize]
    [Produces("application/json")]
    public class WasteReportController : BaseController {

        /// <summary>
        /// Create a new waste report with optional images
        /// </summary>
        /// <param name="command">Waste report details including location, waste type, and images</param>
        /// <returns>Created waste report with uploaded images</returns>
        /// <response code="201">Waste report created successfully</response>
        /// <response code="400">Invalid input data or creation failed</response>
        /// <response code="401">User is not authenticated</response>
        [HttpPost("create")]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(typeof(Response<WasteReportResponse>), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> CreateWasteReport([FromForm] CreateWasteReportCommand command) {
            var result = await Mediator.Send(command);
            return NewResult(result);
        }

        /// <summary>
        /// Get a waste report by its ID
        /// </summary>
        /// <param name="id">Waste report ID</param>
        /// <returns>Waste report details with images and user information</returns>
        /// <response code="200">Returns the waste report details</response>
        /// <response code="404">Waste report not found</response>
        /// <response code="401">User is not authenticated</response>
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(Response<WasteReportResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetWasteReportById([FromRoute] int id) {
            var query = new GetWasteReportByIdQuery { Id = id };
            var result = await Mediator.Send(query);
            return NewResult(result);
        }
    }
}


using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using YallaKhadra.API.Bases;
using YallaKhadra.Core.Bases.Responses;
using YallaKhadra.Core.Enums;
using YallaKhadra.Core.Features.AIWasteScans.Commands.RequestModels;
using YallaKhadra.Core.Features.AIWasteScans.Queries.Models;
using YallaKhadra.Core.Features.AIWasteScans.Queries.Responses;

namespace YallaKhadra.API.Controllers {
    /// <summary>
    /// AI Waste Scan controller for waste identification using AI
    /// </summary>
    [ApiController]
    [Authorize]
    [Produces("application/json")]
    public class AIWasteScanController : BaseController {

        /// <summary>
        /// Create a new AI waste scan by uploading an image
        /// </summary>
        /// <param name="command">Waste image to analyze</param>
        /// <returns>AI scan result with waste type, recyclability, and explanation</returns>
        /// <response code="201">AI scan created successfully with predictions</response>
        /// <response code="400">Invalid image or scan failed</response>
        /// <response code="401">User is not authenticated</response>
        [HttpPost]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(typeof(Response<AIWasteScanResponse>), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> CreateAIWasteScan([FromForm] CreateAIWasteScanCommand command) {
            var result = await Mediator.Send(command);
            return NewResult(result);
        }

        /// <summary>
        /// Get AI waste scan by ID
        /// </summary>
        /// <param name="id">Scan ID</param>
        /// <returns>AI scan details with predictions and image</returns>
        /// <response code="200">Returns the AI scan details</response>
        /// <response code="404">Scan not found or access denied</response>
        /// <response code="401">User is not authenticated</response>
        [HttpGet("{id:int}")]
        [Authorize(Roles = $"{nameof(UserRole.User)},{nameof(UserRole.SuperAdmin)},{nameof(UserRole.Admin)}")]
        [ProducesResponseType(typeof(Response<AIWasteScanResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetAIWasteScanById([FromRoute] int id) {
            var query = new GetAIWasteScanByIdQuery { Id = id };
            var result = await Mediator.Send(query);
            return NewResult(result);
        }

        /// <summary>
        /// Get current user's AI waste scans
        /// </summary>
        /// <returns>List of all scans for the current user, ordered by date (newest first)</returns>
        /// <response code="200">Returns the user's AI scans</response>
        /// <response code="401">User is not authenticated</response>
        [HttpGet("my")]
        [ProducesResponseType(typeof(Response<List<AIWasteScanResponse>>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetMyAIWasteScans() {
            var query = new GetMyAIWasteScansQuery();
            var result = await Mediator.Send(query);
            return NewResult(result);
        }

        /// <summary>
        /// Get all AI waste scans with pagination (Admin only)
        /// </summary>
        /// <param name="query">Pagination parameters (page number and page size)</param>
        /// <returns>Paginated list of all AI scans with full details, ordered by date (newest first)</returns>
        /// <response code="200">Returns paginated list of AI scans</response>
        /// <response code="400">Invalid pagination parameters</response>
        /// <response code="401">User is not authenticated or not an admin</response>
        [HttpGet]
        [Authorize(Roles = $"{nameof(UserRole.Admin)},{nameof(UserRole.SuperAdmin)}")]
        [ProducesResponseType(typeof(PaginatedResult<AIWasteScanResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetAllAIWasteScans([FromQuery] GetAllAIWasteScansQuery query) {
            var result = await Mediator.Send(query);
            return Ok(result);
        }
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using YallaKhadra.API.Bases;
using YallaKhadra.Core.Bases.Responses;
using YallaKhadra.Core.Enums;
using YallaKhadra.Core.Features.CleanupTasks.Commands.RequestModels;
using YallaKhadra.Core.Features.CleanupTasks.Queries.Models;
using YallaKhadra.Core.Features.CleanupTasks.Queries.Responses;

namespace YallaKhadra.API.Controllers {
    /// <summary>
    /// Cleanup Task controller for managing waste cleanup assignments and completion
    /// </summary>
    [ApiController]
    [Produces("application/json")]
    public class CleanupTaskController : BaseController {

        /// <summary>
        /// Get all cleanup tasks with pagination (Admin only)
        /// </summary>
        /// <param name="query">Pagination parameters (page number and page size)</param>
        /// <returns>Paginated list of all cleanup tasks with full details, ordered by assigned date (newest first)</returns>
        /// <response code="200">Returns paginated list of cleanup tasks</response>
        /// <response code="400">Invalid pagination parameters</response>
        /// <response code="401">User is not authenticated or not an admin</response>
        [HttpGet]
        [Authorize(Roles = $"{nameof(UserRole.Admin)},{nameof(UserRole.SuperAdmin)}")]
        [ProducesResponseType(typeof(PaginatedResult<CleanupTaskResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetAllCleanupTasks([FromQuery] GetAllCleanupTasksQuery query) {
            var result = await Mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// Assign a waste report to the current worker for cleanup
        /// </summary>
        /// <param name="command">Report ID to assign</param>
        /// <returns>Created cleanup task with report details</returns>
        /// <response code="201">Cleanup task assigned successfully, report status updated to InProgress</response>
        /// <response code="400">Invalid request or assignment failed</response>
        /// <response code="401">User is not authenticated or not a worker</response>
        /// <response code="404">Report not found</response>
        [HttpPost("assign")]
        [Authorize(Roles = nameof(UserRole.Worker))]
        [ProducesResponseType(typeof(Response<CleanupTaskResponse>), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> AssignCleanupTask([FromBody] AssignCleanupTaskCommand command) {
            var result = await Mediator.Send(command);
            return NewResult(result);
        }

        /// <summary>
        /// Complete a cleanup task with final details and optional images
        /// </summary>
        /// <param name="command">Task completion details including weight, waste type, and images</param>
        /// <returns>Completed cleanup task with all details</returns>
        /// <response code="200">Cleanup task completed successfully, report status updated to Done</response>
        /// <response code="400">Invalid request or completion failed</response>
        /// <response code="401">User is not authenticated, not a worker, or not assigned to this task</response>
        /// <response code="404">Task not found</response>
        [HttpPost("complete")]
        [Authorize(Roles = nameof(UserRole.Worker))]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(typeof(Response<CleanupTaskResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> CompleteCleanupTask([FromForm] CompleteCleanupTaskCommand command) {
            var result = await Mediator.Send(command);
            return NewResult(result);
        }

        /// <summary>
        /// Get current worker's uncompleted cleanup tasks
        /// </summary>
        /// <returns>List of uncompleted tasks assigned to the current worker, ordered by assigned date (newest first)</returns>
        /// <response code="200">Returns the worker's uncompleted tasks</response>
        /// <response code="401">User is not authenticated or not a worker</response>
        [HttpGet("my-uncompleted-tasks")]
        [Authorize(Roles = nameof(UserRole.Worker))]
        [ProducesResponseType(typeof(Response<List<CleanupTaskResponse>>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetMyUncompletedTasks() {
            var query = new GetMyUncompletedTasksQuery();
            var result = await Mediator.Send(query);
            return NewResult(result);
        }
    }
}


using MediatR;
using Microsoft.AspNetCore.Mvc;
using YallaKhadra.API.Bases;
using YallaKhadra.Core.Bases.Authentication;
using YallaKhadra.Core.Bases.Responses;
using YallaKhadra.Core.Features.Authentication.Commands.RequestsModels;
using YallaKhadra.Core.Features.Users.Queries.Models;
using YallaKhadra.Core.Features.Users.Queries.Responses;
using YallaKhadra.Core.Filters;

namespace YallaKhadra.API.Controllers {

    /// <summary>
    /// User management controller for handling user operations
    /// </summary>
    [ApiController]
    [Produces("application/json")]
    public class ApplicationUserController : BaseController {
        public ApplicationUserController(IMediator mediator) : base(mediator) { }

        /// <summary>
        /// Register a new user account
        /// </summary>
        /// <param name="command">User registration details including username, email, password, and personal information</param>
        /// <returns>JWT token with user information if registration is successful</returns>
        /// <response code="201">User registered successfully and JWT token returned</response>
        /// <response code="400">Invalid input data or registration failed</response>
        /// <response code="409">User with the same email, username, or phone number already exists</response>
        /// <response code="403">User is already authenticated (anonymous only endpoint)</response>
        [HttpPost("register")]
        [AnonymousOnly]
        [ProducesResponseType(typeof(Response<AuthResult>), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> Create([FromBody] RegisterCommand command) {
            var result = await mediator.Send(command);
            return NewResult(result);
        }

        /// <summary>
        /// Get all users with pagination
        /// </summary>
        /// <param name="query">Pagination parameters including page number and page size</param>
        /// <returns>Paginated list of users with their details</returns>
        /// <response code="200">Returns paginated list of users</response>
        /// <response code="400">Invalid pagination parameters</response>
        [HttpGet]
        [ProducesResponseType(typeof(PaginatedResult<GetUsersPaginatedResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetAll([FromQuery] GetUsersPaginatedQuery query) {
            var result = await mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// Get user by their unique identifier
        /// </summary>
        /// <param name="query">User ID (GUID)</param>
        /// <returns>User details including username, email, name, address, and phone number</returns>
        /// <response code="200">Returns user details</response>
        /// <response code="404">User not found</response>
        /// <response code="400">Invalid user ID format</response>
        [HttpGet("id/{id:guid}")]
        [ProducesResponseType(typeof(Response<GetUserByIdResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetById([FromRoute] GetUserByIdQuery query) {
            var result = await mediator.Send(query);
            return NewResult(result);
        }

        /// <summary>
        /// Get user by their username
        /// </summary>
        /// <param name="query">Username to search for</param>
        /// <returns>User details including username, full name, and email</returns>
        /// <response code="200">Returns user details</response>
        /// <response code="404">User not found</response>
        /// <response code="400">Invalid username</response>
        [HttpGet("username/{Username}")]
        [ProducesResponseType(typeof(Response<GetUserByUsernameResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetByUsername([FromRoute] GetUserByUsernameQuery query) {
            var result = await mediator.Send(query);
            return NewResult(result);
        }

        ///// <summary>
        ///// Search for users matching specific criteria
        ///// </summary>
        ///// <param name="query">Search parameters including username</param>
        ///// <returns>List of users matching the search criteria</returns>
        ///// <response code="200">Returns list of matching users</response>
        ///// <response code="400">Invalid search parameters</response>
        ///// <response code="401">User not authenticated</response>
        //[Authorize]
        //[HttpGet("search")]
        //[ProducesResponseType(typeof(Response<List<SearchUsersResponse>>), StatusCodes.Status200OK)]
        //[ProducesResponseType(StatusCodes.Status400BadRequest)]
        //[ProducesResponseType(StatusCodes.Status401Unauthorized)]
        //public async Task<IActionResult> SearchUsers([FromQuery] SearchUsersQuery query) {
        //    var result = await mediator.Send(query);
        //    return NewResult(result);
        //}

        /// <summary>
        /// Check if a username is available for registration
        /// </summary>
        /// <param name="query">Username to check</param>
        /// <returns>Boolean indicating whether the username is available</returns>
        /// <response code="200">Returns true if username is available, false otherwise</response>
        /// <response code="400">Invalid username format</response>
        [HttpGet("check-username")]
        [ProducesResponseType(typeof(Response<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CheckUsernameAvailability([FromQuery] CheckUsernameAvailabilityQuery query) {
            var result = await mediator.Send(query);
            return NewResult(result);
        }

        /// <summary>
        /// Check if an email address is available for registration
        /// </summary>
        /// <param name="query">Email address to check</param>
        /// <returns>Boolean indicating whether the email is available</returns>
        /// <response code="200">Returns true if email is available, false otherwise</response>
        /// <response code="400">Invalid email format</response>
        [HttpGet("check-email")]
        [ProducesResponseType(typeof(Response<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CheckEmailAvailability([FromQuery] CheckEmailAvailabilityQuery query) {
            var result = await mediator.Send(query);
            return NewResult(result);
        }



        ///// <summary>
        ///// Update user information
        ///// </summary>
        ///// <param name="id">User ID to update</param>
        ///// <param name="command">Updated user information</param>
        ///// <returns>Updated user details</returns>
        ///// <response code="200">User updated successfully</response>
        ///// <response code="400">Invalid input data</response>
        ///// <response code="404">User not found</response>
        //[HttpPatch("{id:int}")]
        //[ProducesResponseType(StatusCodes.Status200OK)]
        //[ProducesResponseType(StatusCodes.Status400BadRequest)]
        //[ProducesResponseType(StatusCodes.Status404NotFound)]
        //public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdateUserCommand command) {
        //    command.Id = id;
        //    var result = await mediator.Send(command);
        //    return NewResult(result);
        //}

        ///// <summary>
        ///// Delete a user account
        ///// </summary>
        ///// <param name="command">User ID to delete</param>
        ///// <returns>Confirmation of deletion</returns>
        ///// <response code="200">User deleted successfully</response>
        ///// <response code="404">User not found</response>
        ///// <response code="400">Failed to delete user</response>
        //[HttpDelete("{id:int}")]
        //[ProducesResponseType(StatusCodes.Status200OK)]
        //[ProducesResponseType(StatusCodes.Status404NotFound)]
        //[ProducesResponseType(StatusCodes.Status400BadRequest)]
        //public async Task<IActionResult> Delete([FromRoute] DeleteUserByIdCommand command) {
        //    var result = await mediator.Send(command);
        //    return NewResult(result);
        //}

        ///// <summary>
        ///// Update user password
        ///// </summary>
        ///// <param name="id">User ID</param>
        ///// <param name="command">Old and new password information</param>
        ///// <returns>Confirmation of password update</returns>
        ///// <response code="200">Password updated successfully</response>
        ///// <response code="400">Invalid password or update failed</response>
        ///// <response code="404">User not found</response>
        //[HttpPatch("update-password/{id:int}")]
        //[ProducesResponseType(StatusCodes.Status200OK)]
        //[ProducesResponseType(StatusCodes.Status400BadRequest)]
        //[ProducesResponseType(StatusCodes.Status404NotFound)]
        //public async Task<IActionResult> UpdatePassword([FromRoute] int id, [FromBody] ChangePasswordCommand command) {
        //    command.Id = id;
        //    var result = await mediator.Send(command);
        //    return NewResult(result);
        //}

        ///// <summary>
        ///// Reset user password (requires reset password token)
        ///// </summary>
        ///// <param name="command">New password and reset token</param>
        ///// <returns>Confirmation of password reset</returns>
        ///// <response code="200">Password reset successfully</response>
        ///// <response code="400">Invalid token or password</response>
        ///// <response code="401">Invalid or expired reset token</response>
        //[HttpPatch("reset-password")]
        //[Authorize(policy: "ResetPasswordPolicy")]
        //[ProducesResponseType(StatusCodes.Status200OK)]
        //[ProducesResponseType(StatusCodes.Status400BadRequest)]
        //[ProducesResponseType(StatusCodes.Status401Unauthorized)]
        //public async Task<IActionResult> ResetPassword([FromForm] ResetPasswordCommand command) {
        //    var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        //    if (int.TryParse(userIdClaim, out var userId)) {
        //        command.UserId = userId;
        //    }
        //    else {
        //        return Unauthorized("Invalid token or user ID not found.");
        //    }
        //    var result = await mediator.Send(command);
        //    return NewResult(result);
        //}
    }
}

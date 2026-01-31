using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using YallaKhadra.API.Auhtorization;
using YallaKhadra.API.Bases;
using YallaKhadra.API.Filters;
using YallaKhadra.Core.Bases.Authentication;
using YallaKhadra.Core.Bases.Responses;
using YallaKhadra.Core.Enums;
using YallaKhadra.Core.Features.Users.Commands.RequestModels;
using YallaKhadra.Core.Features.Users.Queries.Models;
using YallaKhadra.Core.Features.Users.Queries.Responses;

namespace YallaKhadra.API.Controllers {

    /// <summary>
    /// User management controller for handling user operations
    /// </summary>
    public class UserController : BaseController {
        private readonly IAuthorizationService _authorizationService;
        public UserController(IAuthorizationService authorizationService) {
            _authorizationService = authorizationService;
        }


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
        public async Task<IActionResult> Register([FromForm] RegisterCommand command) {
            var result = await Mediator.Send(command);
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
        [Authorize(Roles = $"{nameof(UserRole.SuperAdmin)},{nameof(UserRole.Admin)}")]
        [ProducesResponseType(typeof(PaginatedResult<GetUsersPaginatedResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetAll([FromQuery] GetUsersPaginatedQuery query) {
            var result = await Mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// Get user by their unique identifier
        /// </summary>
        /// <param name="query">User ID (int)</param>
        /// <returns>User details including username, email, name, address, and phone number</returns>
        /// <response code="200">Returns user details</response>
        /// <response code="404">User not found</response>
        /// <response code="400">Invalid user ID format</response>
        [HttpGet("{id:int}")]
        [Authorize]
        [ProducesResponseType(typeof(Response<GetUserByIdResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetById([FromRoute] int id) {
            var authResult = await _authorizationService.AuthorizeAsync(User, id, AuthorizationPolicies.SameUserOrAdmin);
            if (!authResult.Succeeded)
                return Forbid();

            var result = await Mediator.Send(new GetUserByIdQuery(id));
            return NewResult(result);
        }




        /// <summary>
        /// Check if an email address is available for registration
        /// </summary>
        /// <param name="query">Email address to check</param>
        /// <returns>Boolean indicating whether the email is available</returns>
        /// <response code="200">Returns true if email is available, false otherwise</response>
        /// <response code="400">Invalid email format</response>
        [HttpGet("check-email/{Email}")]
        [ProducesResponseType(typeof(Response<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CheckEmailAvailability([FromRoute] CheckEmailAvailabilityQuery query) {
            var result = await Mediator.Send(query);
            return NewResult(result);
        }




        [HttpPost("add-user")]
        [Authorize(Roles = $"{nameof(UserRole.SuperAdmin)},{nameof(UserRole.Admin)}")]
        public async Task<IActionResult> AddUser([FromBody] AddUserCommand command) {
            var result = await Mediator.Send(command);
            return NewResult(result);
        }


        /// <summary>
        /// Update an existing user
        /// </summary>
        /// <param name="id">User ID</param>
        /// <param name="command">User update details (all fields optional except ID). Profile image can be uploaded as multipart/form-data.</param>
        /// <returns>Confirmation of updated user</returns>
        /// <response code="200">User updated successfully</response>
        /// <response code="400">Invalid input data or update failed</response>
        /// <response code="404">User not found</response>
        /// <response code="401">User is not authenticated</response>
        /// <response code="403">User is not authorized to update this account</response>
        [HttpPatch("{id:int}")]
        [Authorize]
        [ProducesResponseType(typeof(Response), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> Update([FromRoute] int id, [FromForm] UpdateUserCommand command) {
            var authResult = await _authorizationService.AuthorizeAsync(User, id, AuthorizationPolicies.SameUserOrAdmin);
            if (!authResult.Succeeded)
                return Forbid();

            command.Id = id;
            var result = await Mediator.Send(command);
            return NewResult(result);
        }

    }
}

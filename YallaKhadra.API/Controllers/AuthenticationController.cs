using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using YallaKhadra.API.Bases;
using YallaKhadra.API.Filters;
using YallaKhadra.Core.Abstracts.ApiAbstracts;
using YallaKhadra.Core.Bases.Authentication;
using YallaKhadra.Core.Bases.Responses;
using YallaKhadra.Core.Features.Authentication.Commands.RequestsModels;

namespace YallaKhadra.Controllers {

    /// <summary>
    /// Authentication controller for handling user login and token management
    /// </summary>
    [ApiController]
    [Produces("application/json")]
    public class AuthenticationController : BaseController {
        private readonly JwtSettings _jwtSettings;
        private readonly IClientContextService _clientContextService;

        public AuthenticationController(JwtSettings jwtSettings, IClientContextService clientContextService) {
            _jwtSettings = jwtSettings;
            _clientContextService = clientContextService;
        }




        /// <summary>
        /// Authenticate a user with username and password
        /// </summary>
        /// <param name="command">Login credentials including username and password</param>
        /// <returns>JWT access token and user information. Refresh token is stored in HTTP-only cookie</returns>
        /// <response code="200">Login successful, returns access token and user details</response>
        /// <response code="401">Invalid username or password</response>
        /// <response code="403">User is already authenticated (anonymous only endpoint)</response>
        /// <response code="429">Too many login attempts, please try again later</response>
        /// <remarks>
        /// The refresh token is automatically stored in an HTTP-only cookie named 'refreshToken' for security.
        /// Rate limit: 5 attempts per minute per IP address.
        /// </remarks>
        [HttpPost("login")]
        [AnonymousOnly]
        [EnableRateLimiting("loginLimiter")]
        [ProducesResponseType(typeof(Response<AuthResult>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
        public async Task<IActionResult> Login([FromBody] SignInCommand command) {
            var result = await Mediator.Send(command);
            HandleRefreshToken(result);
            return NewResult(result);
        }



        /// <summary>
        /// Refresh an expired access token using the refresh token
        /// </summary>
        /// <param name="command">The current (possibly expired) access token</param>
        /// <returns>New JWT access token and refresh token</returns>
        /// <response code="200">Token refreshed successfully, returns new access token</response>
        /// <response code="401">Invalid or expired refresh token</response>
        /// <response code="400">Invalid access token format</response>
        /// <remarks>
        /// The refresh token is automatically read from the 'refreshToken' HTTP-only cookie.
        /// A new refresh token is generated and stored in the cookie, while the old one is revoked.
        /// The access token in the request body can be expired, but must be valid in format.
        /// </remarks>
        [HttpPost("refresh-token")]
        [ProducesResponseType(typeof(Response<AuthResult>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenCommand command) {
            if (_clientContextService.IsWebClient())
                command.RefreshToken = Request.Cookies["refreshToken"];

            if (command.RefreshToken is null)
                return Unauthorized("Invalid Refresh token.");

            var result = await Mediator.Send(command);

            HandleRefreshToken(result);

            return NewResult(result);
        }




        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout([FromBody] LogoutCommand command) {
            if (_clientContextService.IsWebClient())
                command.RefreshToken = Request.Cookies["refreshToken"];

            if (command.RefreshToken is null)
                return Unauthorized("Refresh token is required");

            var result = await Mediator.Send(command);
            Response.Cookies.Delete("refreshToken");


            return NewResult(result);
        }


        //helpers
        private void HandleRefreshToken(Response<AuthResult> result) {
            if (!result.Succeeded || result.Data?.RefreshToken is null)
                return;

            var refreshToken = result.Data.RefreshToken.Token;

            if (_clientContextService.IsWebClient()) {
                var cookieOptions = new CookieOptions {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.None,
                    Path = "/api/authentication",
                    Expires = result.Data.RefreshToken.ExpirationDate
                };
                Response.Cookies.Append("refreshToken", refreshToken, cookieOptions);
                result.Data.RefreshToken = null;
            }
        }
    }


}



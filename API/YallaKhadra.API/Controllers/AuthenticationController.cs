using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using YallaKhadra.API.Bases;
using YallaKhadra.Core.Abstracts.ServicesContracts;
using YallaKhadra.Core.Bases.Authentication;
using YallaKhadra.Core.Bases.Responses;
using YallaKhadra.Core.Features.Authentication.Commands.RequestsModels;
using YallaKhadra.Core.Filters;

namespace YallaKhadra.Controllers {

    /// <summary>
    /// Authentication controller for handling user login and token management
    /// </summary>
    [ApiController]
    [Produces("application/json")]
    public class AuthenticationController : BaseController {
        private readonly ITokenService _tokenService;

        public AuthenticationController(IMediator mediator, ITokenService tokenService) : base(mediator) {
            _tokenService = tokenService;
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
        [ProducesResponseType(typeof(Response<JwtResult>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
        public async Task<IActionResult> Login([FromBody] SignInCommand command) {
            var result = await mediator.Send(command);
            if (result.Succeeded && result.Data is not null) {
                var refreshtoken = result.Data.RefreshToken!.Token;
                var cookieOptions = _tokenService.GetRefreshTokenCookieOptions();
                Response.Cookies.Append("refreshToken", refreshtoken, cookieOptions);
                result.Data.RefreshToken = null;  //must return another response model without refresh token but i will keep it null for now
            }

            return NewResult(result);
        }

        /// <summary>
        /// Authenticate a user using Google Sign-In
        /// </summary>
        /// <param name="command">Google ID token from Google Sign-In</param>
        /// <returns>JWT access token and user information. Refresh token is stored in HTTP-only cookie</returns>
        /// <response code="200">Google login successful, returns access token and user details</response>
        /// <response code="400">Invalid Google token or failed to create user account</response>
        /// <response code="401">Invalid or expired Google ID token</response>
        /// <response code="403">User is already authenticated (anonymous only endpoint)</response>
        /// <response code="429">Too many login attempts, please try again later</response>
        /// <remarks>
        /// If the user doesn't exist, a new account will be created automatically using Google profile information.
        /// The refresh token is stored in an HTTP-only cookie named 'refreshToken'.
        /// Rate limit: 5 attempts per minute per IP address.
        /// </remarks>
        [HttpPost("google-login")]
        [EnableRateLimiting("loginLimiter")]
        [AnonymousOnly]
        [ProducesResponseType(typeof(Response<JwtResult>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
        public async Task<IActionResult> GoogleLogin([FromBody] GoogleLoginCommand command) {
            var result = await mediator.Send(command);
            if (result.Succeeded && result.Data is not null) {
                var refreshtoken = result.Data.RefreshToken!.Token;
                var cookieOptions = _tokenService.GetRefreshTokenCookieOptions();
                Response.Cookies.Append("refreshToken", refreshtoken, cookieOptions);
                result.Data.RefreshToken = null;
            }
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
        [ProducesResponseType(typeof(Response<JwtResult>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenCommand command) {
            command.RefreshToken = Request.Cookies["refreshToken"];
            var result = await mediator.Send(command);
            if (result.Succeeded && result.Data is not null) {
                var refreshtoken = result.Data.RefreshToken!.Token;
                var cookieOptions = _tokenService.GetRefreshTokenCookieOptions();
                Response.Cookies.Append("refreshToken", refreshtoken, cookieOptions);
                result.Data.RefreshToken = null;
            }
            return NewResult(result);
        }


        ///// <summary>
        ///// Confirm user email address
        ///// </summary>
        ///// <param name="command">User ID and confirmation code from email</param>
        ///// <returns>Confirmation result</returns>
        ///// <response code="200">Email confirmed successfully</response>
        ///// <response code="400">Invalid confirmation code or email already confirmed</response>
        ///// <response code="404">User not found</response>
        //[HttpGet("Confirm-email")]
        //[ProducesResponseType(StatusCodes.Status200OK)]
        //[ProducesResponseType(StatusCodes.Status400BadRequest)]
        //[ProducesResponseType(StatusCodes.Status404NotFound)]
        //public async Task<IActionResult> ConfirmEmail([FromQuery] ConfirmEmailCommand command) {
        //    var result = await mediator.Send(command);
        //    return NewResult(result);
        //}

        ///// <summary>
        ///// Resend email confirmation link
        ///// </summary>
        ///// <param name="command">User email address</param>
        ///// <returns>Confirmation that email was sent</returns>
        ///// <response code="200">Confirmation email sent successfully</response>
        ///// <response code="400">Email already confirmed or failed to send email</response>
        ///// <response code="404">User not found</response>
        //[HttpGet("resend-confirmation-email")]
        //[ProducesResponseType(StatusCodes.Status200OK)]
        //[ProducesResponseType(StatusCodes.Status400BadRequest)]
        //[ProducesResponseType(StatusCodes.Status404NotFound)]
        //public async Task<IActionResult> ResendConfirmationEmail([FromQuery] ResendConfirmationEmailCommand command) {
        //    var result = await mediator.Send(command);
        //    return NewResult(result);
        //}

        ///// <summary>
        ///// Send password reset code to user email
        ///// </summary>
        ///// <param name="command">User email address</param>
        ///// <returns>Confirmation that reset code was sent</returns>
        ///// <response code="200">Password reset code sent successfully</response>
        ///// <response code="400">Failed to send reset code</response>
        ///// <remarks>
        ///// A 6-digit code will be sent to the user's email address, valid for 10 minutes.
        ///// For security reasons, this endpoint always returns success even if the email doesn't exist.
        ///// </remarks>
        //[HttpPost("password-reset/send-email")]
        //[ProducesResponseType(StatusCodes.Status200OK)]
        //[ProducesResponseType(StatusCodes.Status400BadRequest)]
        //public async Task<IActionResult> PasswordResetEmail([FromForm] SendResetPasswordCodeCommand command) {
        //    var result = await _mediator.Send(command);
        //    return NewResult(result);
        //}

        ///// <summary>
        ///// Verify password reset code and get reset token
        ///// </summary>
        ///// <param name="command">Email address and reset code</param>
        ///// <returns>Temporary token for password reset</returns>
        ///// <response code="200">Code verified, returns temporary reset token</response>
        ///// <response code="400">Invalid or expired code</response>
        ///// <response code="404">User not found</response>
        ///// <remarks>
        ///// The returned token is valid for 5 minutes and can only be used for password reset.
        ///// The reset code becomes invalid after verification.
        ///// </remarks>
        //[HttpPost("password-reset/verify-code")]
        //[ProducesResponseType(StatusCodes.Status200OK)]
        //[ProducesResponseType(StatusCodes.Status400BadRequest)]
        //[ProducesResponseType(StatusCodes.Status404NotFound)]
        //public async Task<IActionResult> VerifyPasswordResetCode([FromForm] VerifyResetPasswordCodeCommand command) {
        //    var result = await _mediator.Send(command);
        //    return NewResult(result);
        //}
    }
}
using ahis.template.application.Features.AccountFeatures.Commands;
using ahis.template.application.Shared;
using ahis.template.application.Shared.Mediator;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ahis.template.api.Controllers.v1
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : BaseApiController
    {
        private readonly IMediator _mediator;

        public AccountController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Register a new user
        /// </summary>
        /// <remarks>
        /// This endpoint registers a user using email or username.
        /// An email confirmation link will be sent after successful registration.
        /// </remarks>
        /// <param name="command">User registration data</param>
        /// <response code="200">User registered successfully</response>
        /// <response code="400">Invalid registration request</response>
        /// <response code="500">Unexpected internal server error</response>
        [HttpPost("register")]
        [ProducesResponseType(typeof(ResponseDto<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Produces("application/json")]
        public async Task<IActionResult> Register([FromBody] RegisterUserCommand command)
        {
            return Response(await _mediator.Send(command));
        }

        /// <summary>
        /// Confirm user email
        /// </summary>
        /// <remarks>
        /// Confirms the user's email address using the token sent via email.
        /// </remarks>
        /// <param name="command">Email confirmation payload</param>
        /// <response code="200">Email confirmed successfully</response>
        /// <response code="400">Invalid or expired confirmation token</response>
        /// <response code="500">Unexpected internal server error</response>
        [HttpPost("confirm-email")]
        [ProducesResponseType(typeof(ResponseDto<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Produces("application/json")]
        public async Task<IActionResult> ConfirmEmail([FromBody] ConfirmEmailCommand command)
        {
            return Response(await _mediator.Send(command));
        }

        /// <summary>
        /// Set user password
        /// </summary>
        /// <remarks>
        /// Allows the user to set a password after email verification.
        /// </remarks>
        /// <param name="command">Password setup data</param>
        /// <response code="200">Password set successfully</response>
        /// <response code="400">Invalid password format</response>
        /// <response code="500">Unexpected internal server error</response>
        [HttpPost("set-password")]
        [ProducesResponseType(typeof(ResponseDto<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Produces("application/json")]
        public async Task<IActionResult> SetPassword([FromBody] SetPasswordCommand command)
        {
            return Response(await _mediator.Send(command));
        }

        /// <summary>
        /// Update user profile
        /// </summary>
        /// <remarks>
        /// Updates the authenticated user's profile information.
        /// </remarks>
        /// <param name="command">Profile update data</param>
        /// <response code="200">Profile updated successfully</response>
        /// <response code="400">Invalid profile data</response>
        /// <response code="500">Unexpected internal server error</response>
        [HttpPost("update-profile")]
        [ProducesResponseType(typeof(ResponseDto<UpdateProfileCommand>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Produces("application/json")]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileCommand command)
        {
            return Response(await _mediator.Send(command));
        }

        /// <summary>
        /// Generate authenticator setup
        /// </summary>
        /// <remarks>
        /// Generates QR code and setup key for enabling two-factor authentication (2FA).
        /// </remarks>
        /// <param name="command">Authenticator setup request</param>
        /// <response code="200">Authenticator setup generated successfully</response>
        /// <response code="400">Invalid request</response>
        /// <response code="500">Unexpected internal server error</response>
        [HttpPost("generate-authenticator-setup")]
        [ProducesResponseType(typeof(ResponseDto<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Produces("application/json")]
        public async Task<IActionResult> GenerateAuthenticatorSetup([FromBody] GenerateAuthenticatorSetupCommand command)
        {
            return Response(await _mediator.Send(command));
        }

        /// <summary>
        /// Generate password reset token
        /// </summary>
        /// <remarks>
        /// Generates a password reset token and sends it to the user's registered email.
        /// </remarks>
        /// <param name="command">Password reset token request</param>
        /// <response code="200">Password reset token generated successfully</response>
        /// <response code="400">Invalid email address</response>
        /// <response code="500">Unexpected internal server error</response>
        [HttpPost("generate-password-reset-token")]
        [ProducesResponseType(typeof(ResponseDto<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Produces("application/json")]
        public async Task<IActionResult> GeneratePasswordResetToken([FromBody] GeneratePasswordResetTokenCommand command)
        {
            return Response(await _mediator.Send(command));
        }


        //[HttpPost("enable-authenticator")] 
        //public async Task<IActionResult> EnableAuthenticator([FromBody] VerifyTwoFactorCommand command) 
        //{ 
        // return Response(await _mediator.Send(command)); 
        //}
    }
}

using ahis.template.application.Features.AuthenticationFeatures.Commands;
using ahis.template.application.Features.AuthenticationFeatures.Queries;
using ahis.template.application.Shared;
using ahis.template.application.Shared.Mediator;
using ahis.template.domain.Models.ViewModels.AuthenticationVM;
using ahis.template.identity.Interfaces;
using FluentResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace ahis.template.api.Controllers.v1
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : BaseApiController
    {
        private readonly IMediator _mediator;

        public AuthenticationController(IMediator mediator)
        {
            _mediator = mediator;
        }


        /// <summary>
        /// Checks the current account state using username or email.
        /// </summary>
        /// <remarks>
        /// This endpoint is used for <b>pre-login account validation</b>.
        /// <para>
        /// It does <b>NOT</b> authenticate the user, generate tokens, or validate passwords.
        /// </para>
        /// <para>
        /// The purpose of this endpoint is to allow the frontend to determine
        /// the correct user flow based on account status:
        /// </para>
        /// <list type="bullet">
        ///   <item>
        ///     <description>Whether the email has been confirmed</description>
        ///   </item>
        ///   <item>
        ///     <description>Whether the user has already created a password</description>
        ///   </item>
        ///   <item>
        ///     <description>Whether two-factor authentication (2FA) is enabled</description>
        ///   </item>
        /// </list>
        /// <para>
        /// This endpoint always returns a successful response even if the email
        /// does not exist, to prevent user enumeration attacks.
        /// </para>
        /// <para>
        /// Frontend usage example:
        /// </para>
        /// <list type="number">
        ///   <item>
        ///     <description>If email is not confirmed → prompt email verification</description>
        ///   </item>
        ///   <item>
        ///     <description>If password is not created → redirect to password setup</description>
        ///   </item>
        ///   <item>
        ///     <description>If 2FA is enabled → redirect to 2FA verification</description>
        ///   </item>
        ///   <item>
        ///     <description>Otherwise → show login form</description>
        ///   </item>
        /// </list>
        /// </remarks>
        /// <param name="query">
        /// Request payload containing the user's email.
        /// </param>
        /// <response code="200">
        /// Returns account state information required to determine the next authentication step.
        /// </response>
        /// <response code="400">
        /// Returned when the request payload is invalid.
        /// </response>
        [HttpPost("check-account")]
        [ProducesResponseType(typeof(ResponseDto<CheckAccountStateVM>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
        [EnableRateLimiting("LoginPolicy")]
        public async Task<IActionResult> CheckAccount([FromBody] CheckAccountStateByUsernameOrEmailQuery query)
        {
            var result = await _mediator.Send(query);
            return Response(result);
        }

        /// <summary>
        /// Authenticate a user using username/email and password
        /// </summary>
        /// <remarks>
        /// This endpoint validates the user's credentials and initiates the authentication flow.
        ///
        /// ## Authentication Flow
        /// 1. User submits **username/email** and **password**
        /// 2. If credentials are invalid → authentication fails
        /// 3. If **Two-Factor Authentication (2FA) is enabled**:
        ///    - No access or refresh tokens are issued
        ///    - Response indicates `requiresTwoFactor = true`
        ///    - Frontend must call **Verify 2FA** endpoint
        /// 4. If authentication is fully successful:
        ///    - An **access token** is returned in the response body
        ///    - A **refresh token** is issued as an **HttpOnly Secure cookie**
        ///
        /// ## Refresh Token Behavior
        /// - Stored as an **HttpOnly** cookie (not accessible via JavaScript)
        /// - Automatically sent by the browser on refresh requests
        /// - Scoped to `/api/authentication/refresh`
        ///
        /// ## Notes for Frontend Developers
        /// - If `requiresTwoFactor` is true:
        ///   - Do NOT treat login as complete
        ///   - Prompt user for 2FA code
        ///   - Call `/api/authentication/verify-2fa`
        /// </remarks>
        /// <param name="command">User login credentials</param>
        /// <response code="200">Login succeeded or 2FA is required</response>
        /// <response code="400">Invalid login credentials</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="423">User account is locked</response>
        /// <response code="500">Unexpected internal server error</response>

        [HttpPost("login")]
        [ProducesResponseType(typeof(ResponseDto<AuthenticationResponseVM>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status423Locked)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
        [Produces("application/json")]
        [EnableRateLimiting("LoginPolicy")]
        public async Task<IActionResult> Login(LoginCommand command)
        {

            var result = await _mediator.Send(command);

            if (result.IsFailed)
                return Response(result);

            // Set refresh token as HttpOnly cookie
            if (!result.Value.RequiresTwoFactor)
            {
                HttpContext.Response.Cookies.Append(
                    "refresh_token",
                    result.Value.RefreshToken!,
                    new CookieOptions
                    {
                        HttpOnly = true,
                        Secure = true,
                        SameSite = SameSiteMode.Strict,
                        Expires = result.Value.RefreshTokenExpiresAt,
                        Path = "/api/authentication/refresh"
                    }
                );
            }



            return Response(Result.Ok(new
            {
                accessToken = result.Value.AccessToken,
                expiresInSeconds = result.Value.ExpiresInSeconds,
                userId = result.Value.UserId,
                requiresTwoFactor = result.Value.RequiresTwoFactor
            }).WithSuccess("Successfully logged in"));


        }

        /// <summary>
        /// Verifies a two-factor authentication (2FA) login attempt.
        /// </summary>
        /// <remarks>
        /// This endpoint is called after the user has successfully entered their username and password
        /// and is required to complete two-factor authentication (2FA).
        ///
        /// Flow:
        /// 1. Frontend submits the 2FA verification code (e.g., from Authenticator app).
        /// 2. Backend validates the code against the selected 2FA provider.
        /// 3. If valid:
        ///    - An access token is returned in the response body.
        ///    - A refresh token is securely stored as an HttpOnly cookie.
        /// 4. If invalid or expired, an error response is returned.
        ///
        /// Notes for Frontend:
        /// - The refresh token is NOT returned in the response body.
        /// - It is stored as an HttpOnly cookie and will be automatically sent by the browser
        ///   when calling the refresh-token endpoint.
        /// - The access token should be stored in memory or a secure client-side store
        ///   and attached to subsequent API requests as a Bearer token.
        ///
        /// Expected Errors:
        /// - 400 Bad Request: Invalid or malformed request payload.
        /// - 401 Unauthorized: Invalid or expired 2FA code.
        /// - 423 Locked: Account is locked due to multiple failed attempts.
        /// </remarks>
        /// <param name="command">
        /// 2FA verification payload containing:
        /// - User identifier
        /// - 2FA provider (e.g., Authenticator)
        /// - One-time verification code
        /// </param>
        /// <returns>
        /// Returns an access token and token expiry information upon successful verification.
        /// </returns>
        [HttpPost("verify-2fa")]
        [ProducesResponseType(typeof(ResponseDto<AuthenticationResponseVM>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status423Locked)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Produces("application/json")]
        public async Task<IActionResult> VerifyTwoFactor([FromBody] VerifyTwoFactorLoginCommand command)
        {
            var result = await _mediator.Send(command);

            if (result.IsFailed)
                return Response(result);

            HttpContext.Response.Cookies.Append(
                "refresh_token",
                result.Value.RefreshToken,
                new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                    Expires = result.Value.RefreshTokenExpiresAt,
                    Path = "/api/authentication/refresh"
                });

            return Response(Result.Ok(new
            {
                accessToken = result.Value.AccessToken,
                expiresInSeconds = result.Value.ExpiresInSeconds,
                userId = result.Value.UserId
            }).WithSuccess("Successfully authenticated with two-factor authentication"));
        }

        [HttpPost("logout")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> Logout()
        {
            Request.Cookies.TryGetValue("refresh_token", out var refreshToken);

            await _mediator.Send(new LogoutCommand(refreshToken));

            // Clear cookie
            HttpContext.Response.Cookies.Append(
                "refresh_token",
                string.Empty,
                new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTime.UtcNow.AddDays(-1),
                    Path = "/api/authentication/refresh"
                });

            return Ok(new { message = "Successfully logged out." });
        }


        /// <summary>
        /// Initiates password reset process using email.
        /// </summary>
        /// <remarks>
        /// This endpoint does not indicate whether the email exists.
        /// If the account is valid, a password reset email will be sent.
        /// </remarks>
        [HttpPost("forgot-password")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> ForgotPassword(
            [FromBody] ForgotPasswordCommand command)
        {
            var result = await _mediator.Send(command);
            return Response(result);
        }



        //[HttpPost("refresh")]
        //public async Task<IActionResult> Refresh()
        //{
        //    if (!Request.Cookies.TryGetValue("refresh_token", out var refreshToken))
        //        return Unauthorized("Missing refresh token.");

        //    var userId = User.GetUserIdFromExpiredToken(); // custom helper

        //    var result = await _mediator.Send(
        //        new RefreshTokenCommand(userId, refreshToken));

        //    if (result.IsFailed)
        //        return Response(result);

        //    var auth = result.Value;

        //    // Rotate refresh token
        //    HttpContext.Response.Cookies.Append(
        //        "refresh_token",
        //        auth.RefreshToken,
        //        new CookieOptions
        //        {
        //            HttpOnly = true,
        //            Secure = true,
        //            SameSite = SameSiteMode.Strict,
        //            Expires = auth.RefreshTokenExpiresAt,
        //            Path = "/api/authentication/refresh"
        //        });

        //    auth.RefreshToken = null;

        //    return Ok(new
        //    {
        //        accessToken = auth.AccessToken,
        //        expiresInSeconds = auth.ExpiresInSeconds
        //    });
        //}

        //[HttpPost("decode-token")]
        //public async Task<IActionResult> DecodeToken([FromBody] string token)
        //{
        //    var result = await _mediator.Send(new DecodeTokenQuery(token));
        //    return Response(result);
        //}
    }
}

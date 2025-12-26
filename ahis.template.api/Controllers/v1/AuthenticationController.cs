using ahis.template.application.Features.AuthenticationFeatures.Commands;
using ahis.template.application.Features.AuthenticationFeatures.Queries;
using ahis.template.application.Shared;
using ahis.template.application.Shared.Mediator;
using FluentResults;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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
        /// Authenticate a user and issue access and refresh tokens
        /// </summary>
        /// <remarks>
        /// This endpoint authenticates a user using a username or email and password.
        /// 
        /// If authentication is successful:
        /// - An **access token** is returned in the response body.
        /// - A **refresh token** is issued and stored as an **HttpOnly Secure cookie**.
        /// 
        /// The refresh token is:
        /// - Not accessible via JavaScript
        /// - Automatically sent by the browser on token refresh requests
        /// - Used to obtain a new access token when the current one expires
        /// </remarks>
        /// <param name="command">User login credentials</param>
        /// <response code="200">User authenticated successfully</response>
        /// <response code="400">Invalid login credentials</response>
        /// <response code="401">Unauthorized access</response>
        /// <response code="423">User account is locked</response>
        /// <response code="500">Unexpected internal server error</response>
        [HttpPost("login")]
        [ProducesResponseType(typeof(ResponseDto<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status423Locked)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Produces("application/json")]
        public async Task<IActionResult> Login(LoginCommand command)
        {

            var result = await _mediator.Send(command);

            if (result.IsFailed)
                return Response(result);

            // Set refresh token as HttpOnly cookie
            HttpContext.Response.Cookies.Append(
                "refresh_token",
                result.Value.RefreshToken,
                new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true, // HTTPS only
                    SameSite = SameSiteMode.Strict,
                    Expires = result.Value.RefreshTokenExpiresAt,
                    Path = "/api/authentication/refresh" // limit exposure
                }
            );


            return Response(Result.Ok(new
            {
                accessToken = result.Value.AccessToken,
                expiresInSeconds = result.Value.ExpiresInSeconds,
                userId = result.Value.UserId
            }).WithSuccess("Successfully logged in"));

        }




        //[HttpPost("decode-token")]
        //public async Task<IActionResult> DecodeToken([FromBody] string token)
        //{
        //    var result = await _mediator.Send(new DecodeTokenQuery(token));
        //    return Response(result);
        //}
    }
}

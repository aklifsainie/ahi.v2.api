using ahis.template.application.Features.AuthenticationFeatures.Commands;
using ahis.template.application.Features.AuthenticationFeatures.Queries;
using ahis.template.application.Shared.Mediator;
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

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginCommand command)
        {
            return Response(await _mediator.Send(command));


            //var result = await _mediator.Send(command);

            //if (result.IsFailed)
            //    return Response(result);

            //var auth = result.Value;

            //// 🔐 Set refresh token as HttpOnly cookie
            //Response.Cookies.Append(
            //    "refresh_token",
            //    auth.RefreshToken,
            //    new CookieOptions
            //    {
            //        HttpOnly = true,
            //        Secure = true, // HTTPS only
            //        SameSite = SameSiteMode.Strict,
            //        Expires = auth.RefreshTokenExpiresAt,
            //        Path = "/api/authentication/refresh" // limit exposure
            //    });

            //// ❗ Remove refresh token from response body
            //auth.RefreshToken = null;

            //return Ok(new
            //{
            //    accessToken = auth.AccessToken,
            //    expiresInSeconds = auth.ExpiresInSeconds,
            //    userId = auth.UserId
            //});
        }

        [HttpPost("decode-token")]
        public async Task<IActionResult> DecodeToken([FromBody] string token)
        {
            var result = await _mediator.Send(new DecodeTokenQuery(token));
            return Response(result);
        }
    }
}

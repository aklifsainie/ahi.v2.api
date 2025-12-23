using ahis.template.application.Features.AccountFeatures.Commands;
using ahis.template.application.Shared;
using ahis.template.application.Shared.Mediator;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

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

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterUserCommand command)
        {
            return Response(await _mediator.Send(command));
        }

        [HttpPost("confirm-email")]
        public async Task<IActionResult> ConfirmEmail([FromBody] ConfirmEmailCommand command)
        {
            return Response(await _mediator.Send(command));
        }

        [HttpPost("set-password")]
        public async Task<IActionResult> SetPassword([FromBody] SetPasswordCommand command)
        {
            return Response(await _mediator.Send(command));
        }

        [HttpPost("update-profile")]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileCommand command)
        {
            return Response(await _mediator.Send(command));
        }

        [HttpPost("generate-authenticator-setup")]
        public async Task<IActionResult> GenerateAuthenticatorSetup([FromBody] GenerateAuthenticatorSetupCommand command)
        {
            return Response(await _mediator.Send(command));
        }

        //[HttpPost("enable-authenticator")]
        //public async Task<IActionResult> EnableAuthenticator([FromBody] VerifyTwoFactorCommand command)
        //{
        //    return Response(await _mediator.Send(command));
        //}

        [HttpPost("generate-password-reset-token")]
        public async Task<IActionResult> GeneratePasswordResetToken([FromBody] GeneratePasswordResetTokenCommand command)
        {
            return Response(await _mediator.Send(command));
        }
    }
}

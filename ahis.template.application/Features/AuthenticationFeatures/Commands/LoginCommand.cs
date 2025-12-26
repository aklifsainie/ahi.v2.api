using ahis.template.application.Shared.Mediator;
using ahis.template.identity.Interfaces;
using FluentResults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ahis.template.application.Features.AuthenticationFeatures.Commands
{
    public record LoginCommand(string UserNameOrEmail,string Password,bool RememberMe) : IRequest<Result<AuthResponseDto>>;


    public class LoginCommandHandler : IRequestHandler<LoginCommand, Result<AuthResponseDto>>
    {
        private readonly IAuthenticationService _authenticationService;


        public LoginCommandHandler(IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
        }


        public async Task<Result<AuthResponseDto>> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            var response = await _authenticationService.LoginAsync(
            request.UserNameOrEmail,
            request.Password,
            request.RememberMe);

            return Result.Ok<AuthResponseDto>(response.Value).WithSuccess("User logged in");


        }
    }
}

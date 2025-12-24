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
    public record LoginCommand(string UserNameOrEmail,string Password,bool RememberMe) : IRequest<Result<string>>;


    public class LoginCommandHandler : IRequestHandler<LoginCommand, Result<string>>
    {
        private readonly IAuthenticationService _authenticationService;


        public LoginCommandHandler(IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
        }


        public async Task<Result<string>> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            var response = await _authenticationService.LoginAsync(
            request.UserNameOrEmail,
            request.Password,
            request.RememberMe);

            return Result.Ok<string>(response.Value.AccessToken).WithSuccess("User logged in");
        }
    }
}

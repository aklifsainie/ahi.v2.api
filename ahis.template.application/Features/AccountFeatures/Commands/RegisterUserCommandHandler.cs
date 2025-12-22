using ahis.template.application.Shared.Mediator;
using ahis.template.identity.Interfaces;
using FluentResults;
using Microsoft.Extensions.Logging;

namespace ahis.template.application.Features.AccountFeatures.Commands
{
    public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, Result<object>>
    {
        private readonly IAccountService _accountService;
        private readonly ILogger<RegisterUserCommandHandler> _logger;

        public RegisterUserCommandHandler(IAccountService accountService, ILogger<RegisterUserCommandHandler> logger)
        {
            _accountService = accountService;
            _logger = logger;
        }

        public async Task<Result<object>> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Registering user {Email}", request.Email);
            var res = await _accountService.RegisterAsync(request.Email, request.UserName, request.CallbackBaseUrl);
            if (!res.IsSuccess)
                return Result.Fail<object>(res.Errors.FirstOrDefault()?.Message ?? "Registration failed.");

            return Result.Ok<object>(res.Value);
        }
    }
}
